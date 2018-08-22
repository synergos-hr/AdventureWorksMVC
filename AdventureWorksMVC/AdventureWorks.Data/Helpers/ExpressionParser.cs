using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AdventureWorks.Data.Helpers
{
    internal class ExpressionParser
    {
        private char ch;
        private IDictionary<string, object> externals;
        private static readonly Expression falseLiteral = Expression.Constant(false);
        private ParameterExpression it;
        private static readonly string keywordIif = "iif";
        private static readonly string keywordIt = "it";
        private static readonly string keywordNew = "new";
        private static Dictionary<string, object> keywords;
        private Dictionary<Expression, string> literals;
        private static readonly Expression nullLiteral = Expression.Constant(null);
        private static readonly Type[] predefinedTypes = new Type[] { 
            typeof(object), typeof(bool), typeof(char), typeof(string), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(DateTime), 
            typeof(TimeSpan), typeof(Guid), typeof(Math), typeof(Convert)
         };
        private Dictionary<string, object> symbols;
        private string text;
        private int textLen;
        private int textPos;
        private Token token;
        private static readonly Expression trueLiteral = Expression.Constant(true);
        
        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (keywords == null)
            {
                keywords = CreateKeywords();
            }
            this.symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.literals = new Dictionary<Expression, string>();
            if (parameters != null)
            {
                this.ProcessParameters(parameters);
            }
            if (values != null)
            {
                this.ProcessValues(values);
            }
            this.text = expression;
            this.textLen = this.text.Length;
            this.SetTextPos(0);
            this.NextToken();
        }
        
        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type type2 in type.GetInterfaces())
                {
                    AddInterface(types, type2);
                }
            }
        }
        
        private void AddSymbol(string name, object value)
        {
            if (this.symbols.ContainsKey(name))
            {
                throw this.ParseError("The identifier '{0}' was defined more than once", new object[] { name });
            }
            this.symbols.Add(name, value);
        }
        
        private void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            MethodBase base2;
            Expression[] args = new Expression[] { expr };
            if (this.FindMethod(signatures, "F", false, args, out base2) != 1)
            {
                throw this.ParseError(errorPos, "Operator '{0}' incompatible with operand type '{1}'", new object[] { opName, GetTypeName(args[0].Type) });
            }
            expr = args[0];
        }
        
        private void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            MethodBase base2;
            Expression[] args = new Expression[] { left, right };
            if (this.FindMethod(signatures, "F", false, args, out base2) != 1)
            {
                throw this.IncompatibleOperandsError(opName, left, right, errorPos);
            }
            left = args[0];
            right = args[1];
        }
        
        private static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1 != t2)
            {
                if (s == t1)
                {
                    return 1;
                }
                if (s == t2)
                {
                    return -1;
                }
                bool flag = IsCompatibleWith(t1, t2);
                bool flag2 = IsCompatibleWith(t2, t1);
                if (flag && !flag2)
                {
                    return 1;
                }
                if (flag2 && !flag)
                {
                    return -1;
                }
                if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2))
                {
                    return 1;
                }
                if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1))
                {
                    return -1;
                }
            }
            return 0;
        }
        
        private static Dictionary<string, object> CreateKeywords()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            dictionary.Add("true", trueLiteral);
            dictionary.Add("false", falseLiteral);
            dictionary.Add("null", nullLiteral);
            dictionary.Add(keywordIt, keywordIt);
            dictionary.Add(keywordIif, keywordIif);
            dictionary.Add(keywordNew, keywordNew);
            foreach (Type type in predefinedTypes)
            {
                dictionary.Add(type.Name, type);
            }
            return dictionary;
        }
        
        private Expression CreateLiteral(object value, string text)
        {
            ConstantExpression key = Expression.Constant(value);
            this.literals.Add(key, text);
            return key;
        }
        
        private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            Func<MethodData, bool> predicate = null;
            MethodData[] applicable = (from m in methods
                select new MethodData { MethodBase = m, Parameters = m.GetParameters() } into m
                where this.IsApplicable(m, args)
                select m).ToArray<MethodData>();
            if (applicable.Length > 1)
            {
                if (predicate == null)
                {
                    predicate = m => applicable.All<MethodData>(delegate (MethodData n) {
                        if (m != n)
                        {
                            return IsBetterThan(args, m, n);
                        }
                        return true;
                    });
                }
                applicable = applicable.Where<MethodData>(predicate).ToArray<MethodData>();
            }
            if (applicable.Length == 1)
            {
                MethodData data = applicable[0];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = data.Args[i];
                }
                method = data.MethodBase;
            }
            else
            {
                method = null;
            }
            return applicable.Length;
        }
        
        private static Type FindGenericType(Type generic, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == generic))
                {
                    return type;
                }
                if (generic.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(generic, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
        
        private int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (Type type2 in SelfAndBaseTypes(type))
            {
                MemberInfo[] defaultMembers = type2.GetDefaultMembers();
                if (defaultMembers.Length != 0)
                {
                    IEnumerable<MethodBase> methods = from p in defaultMembers.OfType<PropertyInfo>()
                        select p.GetGetMethod() into m
                        where m != null
                        select m;
                    int num = this.FindBestMethod(methods, args, out method);
                    if (num != 0)
                    {
                        return num;
                    }
                }
            }
            method = null;
            return 0;
        }
        
        private int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
            BindingFlags bindingAttr = (BindingFlags.Public | BindingFlags.DeclaredOnly) | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type type2 in SelfAndBaseTypes(type))
            {
                MemberInfo[] source = type2.FindMembers(MemberTypes.Method, bindingAttr, Type.FilterNameIgnoreCase, methodName);
                int num = this.FindBestMethod(source.Cast<MethodBase>(), args, out method);
                if (num != 0)
                {
                    return num;
                }
            }
            method = null;
            return 0;
        }
        
        private MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            BindingFlags bindingAttr = (BindingFlags.Public | BindingFlags.DeclaredOnly) | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type type2 in SelfAndBaseTypes(type))
            {
                MemberInfo[] infoArray = type2.FindMembers(MemberTypes.Property | MemberTypes.Field, bindingAttr, Type.FilterNameIgnoreCase, memberName);
                if (infoArray.Length != 0)
                {
                    return infoArray[0];
                }
            }
            return null;
        }
        
        private Expression GenerateAdd(Expression left, Expression right)
        {
            if ((left.Type == typeof(string)) && (right.Type == typeof(string)))
            {
                return this.GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }
        
        private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
            {
                throw this.ParseError(errorPos, "The first expression must be of type 'Boolean'", new object[0]);
            }
            if (expr1.Type != expr2.Type)
            {
                Expression expression = (expr2 != nullLiteral) ? this.PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expression2 = (expr1 != nullLiteral) ? this.PromoteExpression(expr2, expr1.Type, true) : null;
                if ((expression == null) || (expression2 != null))
                {
                    if ((expression2 == null) || (expression != null))
                    {
                        string str = (expr1 != nullLiteral) ? expr1.Type.Name : "null";
                        string str2 = (expr2 != nullLiteral) ? expr2.Type.Name : "null";
                        if ((expression != null) && (expression2 != null))
                        {
                            throw this.ParseError(errorPos, "Both of the types '{0}' and '{1}' convert to the other", new object[] { str, str2 });
                        }
                        throw this.ParseError(errorPos, "Neither of the types '{0}' and '{1}' converts to the other", new object[] { str, str2 });
                    }
                    expr2 = expression2;
                }
                else
                {
                    expr1 = expression;
                }
            }
            return Expression.Condition(test, expr1, expr2);
        }
        
        private Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type type2 = expr.Type;
            if (type2 == type)
            {
                return expr;
            }
            if (type2.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(type2) || IsNullableType(type)) && (GetNonNullableType(type2) == GetNonNullableType(type)))
                {
                    return Expression.Convert(expr, type);
                }
                if (((IsNumericType(type2) || IsEnumType(type2)) && IsNumericType(type)) || IsEnumType(type))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }
            if ((!type2.IsAssignableFrom(type) && !type.IsAssignableFrom(type2)) && (!type2.IsInterface && !type.IsInterface))
            {
                throw this.ParseError(errorPos, "A value of type '{0}' cannot be converted to type '{1}'", new object[] { GetTypeName(type2), GetTypeName(type) });
            }
            return Expression.Convert(expr, type);
        }
        
        private Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
        
        private Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThan(left, right);
        }
        
        private Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThanOrEqual(left, right);
        }
        
        private Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThan(left, right);
        }
        
        private Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThanOrEqual(left, right);
        }
        
        private Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }
        
        private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, this.GetStaticMethod(methodName, left, right), new Expression[] { left, right });
        }
        
        private Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(null, typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) }), new Expression[] { left, right });
        }
        
        private Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }
        
        private string GetIdentifier()
        {
            this.ValidateToken(TokenId.Identifier, "Identifier expected");
            string text = this.token.text;
            if ((text.Length > 1) && (text[0] == '@'))
            {
                text = text.Substring(1);
            }
            return text;
        }
        
        private static Type GetNonNullableType(Type type)
        {
            if (!IsNullableType(type))
            {
                return type;
            }
            return type.GetGenericArguments()[0];
        }
        
        private static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return 1;
                    
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        return 2;
                    
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return 3;
                }
            }
            return 0;
        }
        
        private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new Type[] { left.Type, right.Type });
        }
        
        private static string GetTypeName(Type type)
        {
            Type nonNullableType = GetNonNullableType(type);
            string name = nonNullableType.Name;
            if (type != nonNullableType)
            {
                name = name + '?';
            }
            return name;
        }
        
        private Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return this.ParseError(pos, "Operator '{0}' incompatible with operand types '{1}' and '{2}'", new object[] { opName, GetTypeName(left.Type), GetTypeName(right.Type) });
        }
        
        private bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
            {
                return false;
            }
            Expression[] expressionArray = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo info = method.Parameters[i];
                if (info.IsOut)
                {
                    return false;
                }
                Expression expression = this.PromoteExpression(args[i], info.ParameterType, false);
                if (expression == null)
                {
                    return false;
                }
                expressionArray[i] = expression;
            }
            method.Args = expressionArray;
            return true;
        }
        
        private static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            bool flag = false;
            for (int i = 0; i < args.Length; i++)
            {
                int num2 = CompareConversions(args[i].Type, m1.Parameters[i].ParameterType, m2.Parameters[i].ParameterType);
                if (num2 < 0)
                {
                    return false;
                }
                if (num2 > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }
        
        private static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target)
            {
                return true;
            }
            if (!target.IsValueType)
            {
                return target.IsAssignableFrom(source);
            }
            Type nonNullableType = GetNonNullableType(source);
            Type type = GetNonNullableType(target);
            if ((nonNullableType == source) || (type != target))
            {
                TypeCode code = nonNullableType.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType);
                TypeCode code2 = type.IsEnum ? TypeCode.Object : Type.GetTypeCode(type);
                switch (code)
                {
                    case TypeCode.SByte:
                        switch (code2)
                        {
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.Byte:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.Int16:
                        switch (code2)
                        {
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.UInt16:
                        switch (code2)
                        {
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.Int32:
                        switch (code2)
                        {
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.UInt32:
                        switch (code2)
                        {
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.Int64:
                        switch (code2)
                        {
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.UInt64:
                        switch (code2)
                        {
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    
                    case TypeCode.Single:
                        switch (code2)
                        {
                            case TypeCode.Single:
                            case TypeCode.Double:
                                return true;
                        }
                        break;
                    
                    default:
                        if (nonNullableType == type)
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        
        private static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }
        
        private static bool IsNullableType(Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
        
        private static bool IsNumericType(Type type)
        {
            return (GetNumericTypeKind(type) != 0);
        }
        
        private static bool IsPredefinedType(Type type)
        {
            foreach (Type type2 in predefinedTypes)
            {
                if (type2 == type)
                {
                    return true;
                }
            }
            return false;
        }
        
        private static bool IsSignedIntegralType(Type type)
        {
            return (GetNumericTypeKind(type) == 2);
        }
        
        private static bool IsUnsignedIntegralType(Type type)
        {
            return (GetNumericTypeKind(type) == 3);
        }
        
        private void NextChar()
        {
            if (this.textPos < this.textLen)
            {
                this.textPos++;
            }
            this.ch = (this.textPos < this.textLen) ? this.text[this.textPos] : '\0';
        }
        
        private void NextToken()
        {
            TokenId exclamation;
            while (char.IsWhiteSpace(this.ch))
            {
                this.NextChar();
            }
            int textPos = this.textPos;
            switch (this.ch)
            {
                case '!':
                    this.NextChar();
                    if (this.ch != '=')
                    {
                        exclamation = TokenId.Exclamation;
                    }
                    else
                    {
                        this.NextChar();
                        exclamation = TokenId.ExclamationEqual;
                    }
                    break;
                
                case '"':
                case '\'':
                {
                    char ch = this.ch;
                    do
                    {
                        this.NextChar();
                        while ((this.textPos < this.textLen) && (this.ch != ch))
                        {
                            this.NextChar();
                        }
                        if (this.textPos == this.textLen)
                        {
                            throw this.ParseError(this.textPos, "Unterminated string literal", new object[0]);
                        }
                        this.NextChar();
                    }
                    while (this.ch == ch);
                    exclamation = TokenId.StringLiteral;
                    break;
                }
                case '%':
                    this.NextChar();
                    exclamation = TokenId.Percent;
                    break;
                
                case '&':
                    this.NextChar();
                    if (this.ch != '&')
                    {
                        exclamation = TokenId.Amphersand;
                    }
                    else
                    {
                        this.NextChar();
                        exclamation = TokenId.DoubleAmphersand;
                    }
                    break;
                
                case '(':
                    this.NextChar();
                    exclamation = TokenId.OpenParen;
                    break;
                
                case ')':
                    this.NextChar();
                    exclamation = TokenId.CloseParen;
                    break;
                
                case '*':
                    this.NextChar();
                    exclamation = TokenId.Asterisk;
                    break;
                
                case '+':
                    this.NextChar();
                    exclamation = TokenId.Plus;
                    break;
                
                case ',':
                    this.NextChar();
                    exclamation = TokenId.Comma;
                    break;
                
                case '-':
                    this.NextChar();
                    exclamation = TokenId.Minus;
                    break;
                
                case '.':
                    this.NextChar();
                    exclamation = TokenId.Dot;
                    break;
                
                case '/':
                    this.NextChar();
                    exclamation = TokenId.Slash;
                    break;
                
                case ':':
                    this.NextChar();
                    exclamation = TokenId.Colon;
                    break;
                
                case '<':
                    this.NextChar();
                    if (this.ch != '=')
                    {
                        if (this.ch == '>')
                        {
                            this.NextChar();
                            exclamation = TokenId.LessGreater;
                        }
                        else
                        {
                            exclamation = TokenId.LessThan;
                        }
                    }
                    else
                    {
                        this.NextChar();
                        exclamation = TokenId.LessThanEqual;
                    }
                    break;
                
                case '=':
                    this.NextChar();
                    if (this.ch != '=')
                    {
                        exclamation = TokenId.Equal;
                    }
                    else
                    {
                        this.NextChar();
                        exclamation = TokenId.DoubleEqual;
                    }
                    break;
                
                case '>':
                    this.NextChar();
                    if (this.ch != '=')
                    {
                        exclamation = TokenId.GreaterThan;
                    }
                    else
                    {
                        this.NextChar();
                        exclamation = TokenId.GreaterThanEqual;
                    }
                    break;
                
                case '?':
                    this.NextChar();
                    exclamation = TokenId.Question;
                    break;
                
                case '[':
                    this.NextChar();
                    exclamation = TokenId.OpenBracket;
                    break;
                
                case ']':
                    this.NextChar();
                    exclamation = TokenId.CloseBracket;
                    break;
                
                case '|':
                    this.NextChar();
                    if (this.ch == '|')
                    {
                        this.NextChar();
                        exclamation = TokenId.DoubleBar;
                    }
                    else
                    {
                        exclamation = TokenId.Bar;
                    }
                    break;
                
                default:
                    if ((char.IsLetter(this.ch) || (this.ch == '@')) || (this.ch == '_'))
                    {
                        do
                        {
                            this.NextChar();
                        }
                        while (char.IsLetterOrDigit(this.ch) || (this.ch == '_'));
                        exclamation = TokenId.Identifier;
                    }
                    else if (char.IsDigit(this.ch))
                    {
                        exclamation = TokenId.IntegerLiteral;
                        do
                        {
                            this.NextChar();
                        }
                        while (char.IsDigit(this.ch));
                        if (this.ch == '.')
                        {
                            exclamation = TokenId.RealLiteral;
                            this.NextChar();
                            this.ValidateDigit();
                            do
                            {
                                this.NextChar();
                            }
                            while (char.IsDigit(this.ch));
                        }
                        if ((this.ch == 'E') || (this.ch == 'e'))
                        {
                            exclamation = TokenId.RealLiteral;
                            this.NextChar();
                            if ((this.ch == '+') || (this.ch == '-'))
                            {
                                this.NextChar();
                            }
                            this.ValidateDigit();
                            do
                            {
                                this.NextChar();
                            }
                            while (char.IsDigit(this.ch));
                        }
                        if ((this.ch == 'F') || (this.ch == 'f'))
                        {
                            this.NextChar();
                        }
                    }
                    else
                    {
                        if (this.textPos != this.textLen)
                        {
                            throw this.ParseError(this.textPos, "Syntax error '{0}'", new object[] { this.ch });
                        }
                        exclamation = TokenId.End;
                    }
                    break;
            }
            this.token.id = exclamation;
            this.token.text = this.text.Substring(textPos, this.textPos - textPos);
            this.token.pos = textPos;
        }
        
        public Expression Parse(Type resultType)
        {
            int pos = this.token.pos;
            Expression expr = this.ParseExpression();
            if ((resultType != null) && ((expr = this.PromoteExpression(expr, resultType, true)) == null))
            {
                throw this.ParseError(pos, "Expression of type '{0}' expected", new object[] { GetTypeName(resultType) });
            }
            this.ValidateToken(TokenId.End, "Syntax error");
            return expr;
        }
        
        private Expression ParseAdditive()
        {
            Expression left = this.ParseMultiplicative();
            while (((this.token.id == TokenId.Plus) || (this.token.id == TokenId.Minus)) || (this.token.id == TokenId.Amphersand))
            {
                Token token = this.token;
                this.NextToken();
                Expression right = this.ParseMultiplicative();
                switch (token.id)
                {
                    case TokenId.Plus:
                    {
                        if ((left.Type == typeof(string)) || (right.Type == typeof(string)))
                        {
                            break;
                        }
                        this.CheckAndPromoteOperands(typeof(IAddSignatures), token.text, ref left, ref right, token.pos);
                        left = this.GenerateAdd(left, right);
                        continue;
                    }
                    case TokenId.Comma:
                    {
                        continue;
                    }
                    case TokenId.Minus:
                    {
                        this.CheckAndPromoteOperands(typeof(ISubtractSignatures), token.text, ref left, ref right, token.pos);
                        left = this.GenerateSubtract(left, right);
                        continue;
                    }
                    case TokenId.Amphersand:
                        break;
                    
                    default:
                    {
                        continue;
                    }
                }
                left = this.GenerateStringConcat(left, right);
            }
            return left;
        }
        
        private Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            MethodBase base2;
            Type[] typeArray;
            ParameterExpression it = this.it;
            ParameterExpression expression2 = Expression.Parameter(elementType, "");
            this.it = expression2;
            Expression[] args = this.ParseArgumentList();
            this.it = it;
            if (this.FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out base2) != 1)
            {
                throw this.ParseError(errorPos, "No applicable aggregate method '{0}' exists", new object[] { methodName });
            }
            if ((base2.Name == "Min") || (base2.Name == "Max"))
            {
                typeArray = new Type[] { elementType, args[0].Type };
            }
            else
            {
                typeArray = new Type[] { elementType };
            }
            if (args.Length == 0)
            {
                args = new Expression[] { instance };
            }
            else
            {
                args = new Expression[] { instance, Expression.Lambda(args[0], new ParameterExpression[] { expression2 }) };
            }
            return Expression.Call(typeof(Enumerable), base2.Name, typeArray, args);
        }
        
        private Expression[] ParseArgumentList()
        {
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            Expression[] expressionArray = (this.token.id != TokenId.CloseParen) ? this.ParseArguments() : new Expression[0];
            this.ValidateToken(TokenId.CloseParen, "')' or ',' expected");
            this.NextToken();
            return expressionArray;
        }
        
        private Expression[] ParseArguments()
        {
            List<Expression> list = new List<Expression>();
            while (true)
            {
                list.Add(this.ParseExpression());
                if (this.token.id != TokenId.Comma)
                {
                    break;
                }
                this.NextToken();
            }
            return list.ToArray();
        }
        
        private Expression ParseComparison()
        {
            Expression left = this.ParseAdditive();
            while ((((this.token.id == TokenId.Equal) || (this.token.id == TokenId.DoubleEqual)) || ((this.token.id == TokenId.ExclamationEqual) || (this.token.id == TokenId.LessGreater))) || (((this.token.id == TokenId.GreaterThan) || (this.token.id == TokenId.GreaterThanEqual)) || ((this.token.id == TokenId.LessThan) || (this.token.id == TokenId.LessThanEqual))))
            {
                Token token = this.token;
                this.NextToken();
                Expression right = this.ParseAdditive();
                bool flag = (((token.id == TokenId.Equal) || (token.id == TokenId.DoubleEqual)) || (token.id == TokenId.ExclamationEqual)) || (token.id == TokenId.LessGreater);
                if ((flag && !left.Type.IsValueType) && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
                    {
                        if (!left.Type.IsAssignableFrom(right.Type))
                        {
                            if (!right.Type.IsAssignableFrom(left.Type))
                            {
                                throw this.IncompatibleOperandsError(token.text, left, right, token.pos);
                            }
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression expression3 = this.PromoteExpression(right, left.Type, true);
                        if (expression3 == null)
                        {
                            expression3 = this.PromoteExpression(left, right.Type, true);
                            if (expression3 == null)
                            {
                                throw this.IncompatibleOperandsError(token.text, left, right, token.pos);
                            }
                            left = expression3;
                        }
                        else
                        {
                            right = expression3;
                        }
                    }
                }
                else
                {
                    this.CheckAndPromoteOperands(flag ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), token.text, ref left, ref right, token.pos);
                }
                switch (token.id)
                {
                    case TokenId.LessThan:
                        left = this.GenerateLessThan(left, right);
                        break;
                    
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = this.GenerateEqual(left, right);
                        break;
                    
                    case TokenId.GreaterThan:
                        left = this.GenerateGreaterThan(left, right);
                        break;
                    
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = this.GenerateNotEqual(left, right);
                        break;
                    
                    case TokenId.LessThanEqual:
                        left = this.GenerateLessThanEqual(left, right);
                        break;
                    
                    case TokenId.GreaterThanEqual:
                        left = this.GenerateGreaterThanEqual(left, right);
                        break;
                }
            }
            return left;
        }
        
        private Expression ParseElementAccess(Expression expr)
        {
            MethodBase base2;
            int pos = this.token.pos;
            this.ValidateToken(TokenId.OpenBracket, "'(' expected");
            this.NextToken();
            Expression[] args = this.ParseArguments();
            this.ValidateToken(TokenId.CloseBracket, "']' or ',' expected");
            this.NextToken();
            if (expr.Type.IsArray)
            {
                if ((expr.Type.GetArrayRank() != 1) || (args.Length != 1))
                {
                    throw this.ParseError(pos, "Indexing of multi-dimensional arrays is not supported", new object[0]);
                }
                Expression index = this.PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                {
                    throw this.ParseError(pos, "Array index must be an integer expression", new object[0]);
                }
                return Expression.ArrayIndex(expr, index);
            }
            switch (this.FindIndexer(expr.Type, args, out base2))
            {
                case 0:
                    throw this.ParseError(pos, "No applicable indexer exists in type '{0}'", new object[] { GetTypeName(expr.Type) });
                
                case 1:
                    return Expression.Call(expr, (MethodInfo) base2, args);
            }
            throw this.ParseError(pos, "Ambiguous invocation of indexer in type '{0}'", new object[] { GetTypeName(expr.Type) });
        }
        
        private static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] infoArray = type.FindMembers(MemberTypes.Field, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, Type.FilterNameIgnoreCase, name);
                if (infoArray.Length != 0)
                {
                    return ((FieldInfo) infoArray[0]).GetValue(null);
                }
            }
            return null;
        }
        
        private bool ParseEnumType(out object value)
        {
            value = null;
            this.ValidateToken(TokenId.Identifier);
            Type type = null;
            int pos = this.token.pos;
            string text = this.token.text;
            while (type == null)
            {
                type = Type.GetType(text, false, true);
                if (type == null)
                {
                    this.NextToken();
                    if (this.token.id != TokenId.Dot)
                    {
                        break;
                    }
                    text = text + this.token.text;
                    this.NextToken();
                    if (this.token.id != TokenId.Identifier)
                    {
                        break;
                    }
                    text = text + this.token.text;
                }
            }
            if ((type != null) && IsEnumType(type))
            {
                this.NextToken();
                this.ValidateToken(TokenId.Dot, "'.' expected");
                this.NextToken();
                this.ValidateToken(TokenId.Identifier, "Identifier expected");
                value = Enum.Parse(type, this.token.text, true);
                return true;
            }
            this.SetTextPos(pos);
            this.NextToken();
            return false;
        }
        
        private Exception ParseError(string format, params object[] args)
        {
            return this.ParseError(this.token.pos, format, args);
        }
        
        private Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }
        
        private Expression ParseExpression()
        {
            int pos = this.token.pos;
            Expression test = this.ParseLogicalOr();
            if (this.token.id == TokenId.Question)
            {
                this.NextToken();
                Expression expression2 = this.ParseExpression();
                this.ValidateToken(TokenId.Colon, "':' expected");
                this.NextToken();
                Expression expression3 = this.ParseExpression();
                test = this.GenerateConditional(test, expression2, expression3, pos);
            }
            return test;
        }
        
        private Expression ParseIdentifier()
        {
            object obj2;

            ValidateToken(TokenId.Identifier);

            if (keywords.TryGetValue(token.text, out obj2))
            {
                if (obj2 is Type)
                    return ParseTypeAccess((Type) obj2);

#pragma warning disable 252,253
                if (obj2 == keywordIt)
                    return ParseIt();

                if (obj2 == keywordIif)
                    return ParseIif();

                if (obj2 == keywordNew)
                    return ParseNew();
#pragma warning restore 252,253

                NextToken();

                return (Expression) obj2;
            }

            if (symbols.TryGetValue(token.text, out obj2) || ((externals != null) && externals.TryGetValue(token.text, out obj2)))
            {
                Expression expression = obj2 as Expression;

                if (expression == null)
                {
                    expression = Expression.Constant(obj2);
                }
                else
                {
                    LambdaExpression lambda = expression as LambdaExpression;

                    if (lambda != null)
                        return ParseLambdaInvocation(lambda);
                }

                NextToken();

                return expression;
            }

            if (ParseEnumType(out obj2))
            {
                Expression expression3 = Expression.Constant(obj2);
                NextToken();
                return expression3;
            }

            if (it == null)
                throw ParseError("Unknown identifier '{0}'", new object[] {this.token.text});

            return ParseMemberAccess(null, it);
        }
        
        private Expression ParseIif()
        {
            int pos = this.token.pos;
            this.NextToken();
            Expression[] expressionArray = this.ParseArgumentList();
            if (expressionArray.Length != 3)
            {
                throw this.ParseError(pos, "The 'iif' function requires three arguments", new object[0]);
            }
            return this.GenerateConditional(expressionArray[0], expressionArray[1], expressionArray[2], pos);
        }
        
        private Expression ParseIntegerLiteral()
        {
            long num2;
            this.ValidateToken(TokenId.IntegerLiteral);
            string text = this.token.text;
            if (text[0] != '-')
            {
                ulong num;
                if (!ulong.TryParse(text, out num))
                {
                    throw this.ParseError("Invalid integer literal '{0}'", new object[] { text });
                }
                this.NextToken();
                if (num <= 0x7fffffffL)
                {
                    return this.CreateLiteral((int) num, text);
                }
                if (num <= 0xffffffffL)
                {
                    return this.CreateLiteral((uint) num, text);
                }
                if (num <= 0x7fffffffffffffffL)
                {
                    return this.CreateLiteral((long) num, text);
                }
                return this.CreateLiteral(num, text);
            }
            if (!long.TryParse(text, out num2))
            {
                throw this.ParseError("Invalid integer literal '{0}'", new object[] { text });
            }
            this.NextToken();
            if ((num2 >= -2147483648L) && (num2 <= 0x7fffffffL))
            {
                return this.CreateLiteral((int) num2, text);
            }
            return this.CreateLiteral(num2, text);
        }
        
        private Expression ParseIt()
        {
            if (this.it == null)
            {
                throw this.ParseError("No 'it' is in scope", new object[0]);
            }
            this.NextToken();
            return this.it;
        }
        
        private Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            MethodBase base2;
            int pos = this.token.pos;
            this.NextToken();
            Expression[] args = this.ParseArgumentList();
            if (this.FindMethod(lambda.Type, "Invoke", false, args, out base2) != 1)
            {
                throw this.ParseError(pos, "Argument list incompatible with lambda expression", new object[0]);
            }
            return Expression.Invoke(lambda, args);
        }
        
        private Expression ParseLogicalAnd()
        {
            Expression left = this.ParseComparison();
            while ((this.token.id == TokenId.DoubleAmphersand) || this.TokenIdentifierIs("and"))
            {
                Token token = this.token;
                this.NextToken();
                Expression right = this.ParseComparison();
                this.CheckAndPromoteOperands(typeof(ILogicalSignatures), token.text, ref left, ref right, token.pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }
        
        private Expression ParseLogicalOr()
        {
            Expression left = this.ParseLogicalAnd();
            while ((this.token.id == TokenId.DoubleBar) || this.TokenIdentifierIs("or"))
            {
                Token token = this.token;
                this.NextToken();
                Expression right = this.ParseLogicalAnd();
                this.CheckAndPromoteOperands(typeof(ILogicalSignatures), token.text, ref left, ref right, token.pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }
        
        private Expression ParseMemberAccess(Type type, Expression instance)
        {
            MethodBase base2;
            if (instance != null)
            {
                type = instance.Type;
            }
            int pos = this.token.pos;
            string identifier = this.GetIdentifier();
            this.NextToken();
            if (this.token.id != TokenId.OpenParen)
            {
                MemberInfo info2 = this.FindPropertyOrField(type, identifier, instance == null);
                if (info2 == null)
                {
                    throw this.ParseError(pos, "No property or field '{0}' exists in type '{1}'", new object[] { identifier, GetTypeName(type) });
                }
                if (info2 is PropertyInfo)
                {
                    return Expression.Property(instance, (PropertyInfo) info2);
                }
                return Expression.Field(instance, (FieldInfo) info2);
            }
            if ((instance != null) && (type != typeof(string)))
            {
                Type type2 = FindGenericType(typeof(IEnumerable<>), type);
                if (type2 != null)
                {
                    Type elementType = type2.GetGenericArguments()[0];
                    return this.ParseAggregate(instance, elementType, identifier, pos);
                }
            }
            Expression[] args = this.ParseArgumentList();
            switch (this.FindMethod(type, identifier, instance == null, args, out base2))
            {
                case 0:
                    throw this.ParseError(pos, "No applicable method '{0}' exists in type '{1}'", new object[] { identifier, GetTypeName(type) });
                
                case 1:
                {
                    MethodInfo method = (MethodInfo) base2;
                    if (!IsPredefinedType(method.DeclaringType))
                    {
                        throw this.ParseError(pos, "Methods on type '{0}' are not accessible", new object[] { GetTypeName(method.DeclaringType) });
                    }
                    if (method.ReturnType == typeof(void))
                    {
                        throw this.ParseError(pos, "Method '{0}' in type '{1}' does not return a value", new object[] { identifier, GetTypeName(method.DeclaringType) });
                    }
                    return Expression.Call(instance, method, args);
                }
            }
            throw this.ParseError(pos, "Ambiguous invocation of method '{0}' in type '{1}'", new object[] { identifier, GetTypeName(type) });
        }
        
        private Expression ParseMultiplicative()
        {
            Expression left = this.ParseUnary();
            while (((this.token.id == TokenId.Asterisk) || (this.token.id == TokenId.Slash)) || ((this.token.id == TokenId.Percent) || this.TokenIdentifierIs("mod")))
            {
                Token token = this.token;
                this.NextToken();
                Expression right = this.ParseUnary();
                this.CheckAndPromoteOperands(typeof(IArithmeticSignatures), token.text, ref left, ref right, token.pos);
                switch (token.id)
                {
                    case TokenId.Identifier:
                    case TokenId.Percent:
                        goto Label_0075;
                    
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                }
                continue;
            Label_0075:
                left = Expression.Modulo(left, right);
            }
            return left;
        }
        
        private Expression ParseNew()
        {
            int num;
            string identifier;
            this.NextToken();
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            List<DynamicProperty> properties = new List<DynamicProperty>();
            List<Expression> list2 = new List<Expression>();
        Label_0025:
            num = this.token.pos;
            Expression item = this.ParseExpression();
            if (this.TokenIdentifierIs("as"))
            {
                this.NextToken();
                identifier = this.GetIdentifier();
                this.NextToken();
            }
            else
            {
                MemberExpression expression2 = item as MemberExpression;
                if (expression2 == null)
                {
                    throw ParseError(num, "Expression is missing an 'as' clause", new object[0]);
                }
                identifier = expression2.Member.Name;
            }
            list2.Add(item);
            properties.Add(new DynamicProperty(identifier, item.Type));
            if (this.token.id == TokenId.Comma)
            {
                this.NextToken();
                goto Label_0025;
            }
            this.ValidateToken(TokenId.CloseParen, "')' or ',' expected");
            this.NextToken();
            Type type = DynamicExpression.CreateClass(properties);
            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
            {
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), list2[i]);
            }
            return Expression.MemberInit(Expression.New(type), bindings);
        }
        
        private static object ParseNumber(string text, Type type)
        {
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte num;
                    if (!sbyte.TryParse(text, out num))
                    {
                        break;
                    }
                    return num;
                
                case TypeCode.Byte:
                    byte num2;
                    if (!byte.TryParse(text, out num2))
                    {
                        break;
                    }
                    return num2;
                
                case TypeCode.Int16:
                    short num3;
                    if (!short.TryParse(text, out num3))
                    {
                        break;
                    }
                    return num3;
                
                case TypeCode.UInt16:
                    ushort num4;
                    if (!ushort.TryParse(text, out num4))
                    {
                        break;
                    }
                    return num4;
                
                case TypeCode.Int32:
                    int num5;
                    if (!int.TryParse(text, out num5))
                    {
                        break;
                    }
                    return num5;
                
                case TypeCode.UInt32:
                    uint num6;
                    if (!uint.TryParse(text, out num6))
                    {
                        break;
                    }
                    return num6;
                
                case TypeCode.Int64:
                    long num7;
                    if (!long.TryParse(text, out num7))
                    {
                        break;
                    }
                    return num7;
                
                case TypeCode.UInt64:
                    ulong num8;
                    if (!ulong.TryParse(text, out num8))
                    {
                        break;
                    }
                    return num8;
                
                case TypeCode.Single:
                    float num9;
                    if (!float.TryParse(text, out num9))
                    {
                        break;
                    }
                    return num9;
                
                case TypeCode.Double:
                    double num10;
                    if (!double.TryParse(text, out num10))
                    {
                        break;
                    }
                    return num10;
                
                case TypeCode.Decimal:
                    decimal num11;
                    if (!decimal.TryParse(text, out num11))
                    {
                        break;
                    }
                    return num11;
            }
            return null;
        }
        
        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            Expression expression;
            List<DynamicOrdering> list = new List<DynamicOrdering>();
        Label_0006:
            expression = this.ParseExpression();
            bool flag = true;
            if (this.TokenIdentifierIs("asc") || this.TokenIdentifierIs("ascending"))
            {
                this.NextToken();
            }
            else if (this.TokenIdentifierIs("desc") || this.TokenIdentifierIs("descending"))
            {
                this.NextToken();
                flag = false;
            }
            DynamicOrdering item = new DynamicOrdering {
                Selector = expression,
                Ascending = flag
            };
            list.Add(item);
            if (this.token.id == TokenId.Comma)
            {
                this.NextToken();
                goto Label_0006;
            }
            this.ValidateToken(TokenId.End, "Syntax error");
            return list;
        }
        
        private Expression ParseParenExpression()
        {
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            Expression expression = this.ParseExpression();
            this.ValidateToken(TokenId.CloseParen, "')' or operator expected");
            this.NextToken();
            return expression;
        }
        
        private Expression ParsePrimary()
        {
            Expression instance = this.ParsePrimaryStart();
        Label_0007:
            while (this.token.id == TokenId.Dot)
            {
                this.NextToken();
                instance = this.ParseMemberAccess(null, instance);
            }
            if (this.token.id == TokenId.OpenBracket)
            {
                instance = this.ParseElementAccess(instance);
                goto Label_0007;
            }
            return instance;
        }
        
        private Expression ParsePrimaryStart()
        {
            switch (this.token.id)
            {
                case TokenId.Identifier:
                    return this.ParseIdentifier();
                
                case TokenId.StringLiteral:
                    return this.ParseStringLiteral();
                
                case TokenId.IntegerLiteral:
                    return this.ParseIntegerLiteral();
                
                case TokenId.RealLiteral:
                    return this.ParseRealLiteral();
                
                case TokenId.OpenParen:
                    return this.ParseParenExpression();
            }
            throw this.ParseError("Expression expected", new object[0]);
        }
        
        private Expression ParseRealLiteral()
        {
            this.ValidateToken(TokenId.RealLiteral);
            string text = this.token.text;
            object obj2 = null;
            switch (text[text.Length - 1])
            {
                case 'F':
                case 'f':
                    float num;
                    if (float.TryParse(text.Substring(0, text.Length - 1), out num))
                    {
                        obj2 = num;
                    }
                    break;
                
                default:
                    double num2;
                    if (double.TryParse(text, out num2))
                    {
                        obj2 = num2;
                    }
                    break;
            }
            if (obj2 == null)
            {
                throw this.ParseError("Invalid real literal '{0}'", new object[] { text });
            }
            this.NextToken();
            return this.CreateLiteral(obj2, text);
        }
        
        private Expression ParseStringLiteral()
        {
            this.ValidateToken(TokenId.StringLiteral);
            char ch = this.token.text[0];
            string text = this.token.text.Substring(1, this.token.text.Length - 2);
            int startIndex = 0;
            while (true)
            {
                int index = text.IndexOf(ch, startIndex);
                if (index < 0)
                {
                    break;
                }
                text = text.Remove(index, 1);
                startIndex = index + 1;
            }
            if (ch == '\'')
            {
                if (text.Length != 1)
                {
                    throw this.ParseError("Character literal must contain exactly one character", new object[0]);
                }
                this.NextToken();
                return this.CreateLiteral(text[0], text);
            }
            this.NextToken();
            return this.CreateLiteral(text, text);
        }
        
        private Expression ParseTypeAccess(Type type)
        {
            MethodBase base2;
            int pos = this.token.pos;
            this.NextToken();
            if (this.token.id == TokenId.Question)
            {
                if (!type.IsValueType || IsNullableType(type))
                {
                    throw this.ParseError(pos, "Type '{0}' has no nullable form", new object[] { GetTypeName(type) });
                }
                type = typeof(Nullable<>).MakeGenericType(new Type[] { type });
                this.NextToken();
            }
            if (this.token.id != TokenId.OpenParen)
            {
                this.ValidateToken(TokenId.Dot, "'.' or '(' expected");
                this.NextToken();
                return this.ParseMemberAccess(type, null);
            }
            Expression[] args = this.ParseArgumentList();
            switch (this.FindBestMethod(type.GetConstructors(), args, out base2))
            {
                case 0:
                    if (args.Length != 1)
                    {
                        throw this.ParseError(pos, "No matching constructor in type '{0}'", new object[] { GetTypeName(type) });
                    }
                    return this.GenerateConversion(args[0], type, pos);
                
                case 1:
                    return Expression.New((ConstructorInfo) base2, args);
            }
            throw this.ParseError(pos, "Ambiguous invocation of '{0}' constructor", new object[] { GetTypeName(type) });
        }
        
        private Expression ParseUnary()
        {
            if (((this.token.id != TokenId.Minus) && (this.token.id != TokenId.Exclamation)) && !this.TokenIdentifierIs("not"))
            {
                return this.ParsePrimary();
            }
            Token token = this.token;
            this.NextToken();
            if ((token.id == TokenId.Minus) && ((this.token.id == TokenId.IntegerLiteral) || (this.token.id == TokenId.RealLiteral)))
            {
                this.token.text = "-" + this.token.text;
                this.token.pos = token.pos;
                return this.ParsePrimary();
            }
            Expression expr = this.ParseUnary();
            if (token.id == TokenId.Minus)
            {
                this.CheckAndPromoteOperand(typeof(INegationSignatures), token.text, ref expr, token.pos);
                return Expression.Negate(expr);
            }
            this.CheckAndPromoteOperand(typeof(INotSignatures), token.text, ref expr, token.pos);
            return Expression.Not(expr);
        }
        
        private void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression expression in parameters)
            {
                if (!string.IsNullOrEmpty(expression.Name))
                {
                    this.AddSymbol(expression.Name, expression);
                }
            }
            if ((parameters.Length == 1) && string.IsNullOrEmpty(parameters[0].Name))
            {
                this.it = parameters[0];
            }
        }
        
        private void ProcessValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object obj2 = values[i];
                if ((i == (values.Length - 1)) && (obj2 is IDictionary<string, object>))
                {
                    this.externals = (IDictionary<string, object>) obj2;
                }
                else
                {
                    this.AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), obj2);
                }
            }
        }
        
        private Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type)
            {
                return expr;
            }
            if (expr is ConstantExpression)
            {
                ConstantExpression key = (ConstantExpression) expr;
                if (key == nullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    string str;
                    if (this.literals.TryGetValue(key, out str))
                    {
                        Type nonNullableType = GetNonNullableType(type);
                        object obj2 = null;
                        switch (Type.GetTypeCode(key.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                obj2 = ParseNumber(str, nonNullableType);
                                break;
                            
                            case TypeCode.Double:
                                if (nonNullableType == typeof(decimal))
                                {
                                    obj2 = ParseNumber(str, nonNullableType);
                                }
                                break;
                            
                            case TypeCode.String:
                                obj2 = ParseEnum(str, nonNullableType);
                                break;
                        }
                        if (obj2 != null)
                        {
                            return Expression.Constant(obj2, type);
                        }
                    }
                }
            }
            if (!IsCompatibleWith(expr.Type, type))
            {
                return null;
            }
            if (!type.IsValueType && !exact)
            {
                return expr;
            }
            return Expression.Convert(expr, type);
        }
        
        private static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (true)
            {
                if (type == null)
                {
                    yield break;
                }
                yield return type;
                type = type.BaseType;
            }
        }
        
        private static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }
        
        private void SetTextPos(int pos)
        {
            this.textPos = pos;
            this.ch = (this.textPos < this.textLen) ? this.text[this.textPos] : '\0';
        }
        
        private bool TokenIdentifierIs(string id)
        {
            return ((this.token.id == TokenId.Identifier) && string.Equals(id, this.token.text, StringComparison.OrdinalIgnoreCase));
        }
        
        private void ValidateDigit()
        {
            if (!char.IsDigit(this.ch))
            {
                throw this.ParseError(this.textPos, "Digit expected", new object[0]);
            }
        }
        
        private void ValidateToken(TokenId t)
        {
            if (this.token.id != t)
            {
                throw this.ParseError("Syntax error", new object[0]);
            }
        }
        
        private void ValidateToken(TokenId t, string errorMessage)
        {
            if (this.token.id != t)
            {
                throw this.ParseError(errorMessage, new object[0]);
            }
        }
        
        private interface IAddSignatures : ExpressionParser.IArithmeticSignatures
        {
            void F(DateTime x, TimeSpan y);
            void F(TimeSpan x, TimeSpan y);
            void F(DateTime? x, TimeSpan? y);
            void F(TimeSpan? x, TimeSpan? y);
        }
        
        private interface IArithmeticSignatures
        {
            void F(decimal x, decimal y);
            void F(double x, double y);
            void F(int x, int y);
            void F(long x, long y);
            void F(float x, float y);
            void F(uint x, uint y);
            void F(ulong x, ulong y);
            void F(decimal? x, decimal? y);
            void F(double? x, double? y);
            void F(int? x, int? y);
            void F(long? x, long? y);
            void F(float? x, float? y);
            void F(uint? x, uint? y);
            void F(ulong? x, ulong? y);
        }
        
        private interface IEnumerableSignatures
        {
            void All(bool predicate);
            void Any();
            void Any(bool predicate);
            void Average(decimal selector);
            void Average(double selector);
            void Average(int selector);
            void Average(long selector);
            void Average(float selector);
            void Average(decimal? selector);
            void Average(double? selector);
            void Average(int? selector);
            void Average(long? selector);
            void Average(float? selector);
            void Count();
            void Count(bool predicate);
            void Max(object selector);
            void Min(object selector);
            void Sum(decimal selector);
            void Sum(double selector);
            void Sum(int selector);
            void Sum(long selector);
            void Sum(decimal? selector);
            void Sum(double? selector);
            void Sum(int? selector);
            void Sum(long? selector);
            void Sum(float? selector);
            void Sum(float selector);
            void Where(bool predicate);
        }
        
        private interface IEqualitySignatures : ExpressionParser.IRelationalSignatures, ExpressionParser.IArithmeticSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);
        }
        
        private interface ILogicalSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);
        }
        
        private interface INegationSignatures
        {
            void F(decimal x);
            void F(double x);
            void F(int x);
            void F(long x);
            void F(float x);
            void F(decimal? x);
            void F(double? x);
            void F(int? x);
            void F(long? x);
            void F(float? x);
        }
        
        private interface INotSignatures
        {
            void F(bool x);
            void F(bool? x);
        }
        
        private interface IRelationalSignatures : ExpressionParser.IArithmeticSignatures
        {
            void F(char x, char y);
            void F(DateTime x, DateTime y);
            void F(string x, string y);
            void F(TimeSpan x, TimeSpan y);
            void F(char? x, char? y);
            void F(DateTime? x, DateTime? y);
            void F(TimeSpan? x, TimeSpan? y);
        }
        
        private interface ISubtractSignatures : ExpressionParser.IAddSignatures, ExpressionParser.IArithmeticSignatures
        {
            void F(DateTime x, DateTime y);
            void F(DateTime? x, DateTime? y);
        }
        
        private class MethodData
        {
            public Expression[] Args;
            public System.Reflection.MethodBase MethodBase;
            public ParameterInfo[] Parameters;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct Token
        {
            public ExpressionParser.TokenId id;
            public string text;
            public int pos;
        }
        
        private enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar
        }
    }
}
