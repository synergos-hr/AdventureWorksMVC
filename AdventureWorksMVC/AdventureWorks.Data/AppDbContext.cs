using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using NLog;
using AdventureWorks.Data.Entity.Configuration;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Interfaces;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Data.Entity.Views.Users;

namespace AdventureWorks.Data
{
    public partial class AppDbContext : DbContext
    {
        #region Properties

        public int UserId { get; set; }
        public string UserName { get; set; }

        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        #endregion Properties

        public AppDbContext()
            : base("name=DefaultConnection")
        {
        }

        #region DbSet

        #region AspNet

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        #endregion

        //public virtual DbSet<AWBuildVersion> AWBuildVersions { get; set; }
        //public virtual DbSet<DatabaseLog> DatabaseLogs { get; set; }
        //public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; }
        public virtual DbSet<EmployeePayHistory> EmployeePayHistories { get; set; }
        public virtual DbSet<JobCandidate> JobCandidates { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<BusinessEntity> BusinessEntities { get; set; }
        public virtual DbSet<BusinessEntityAddress> BusinessEntityAddresses { get; set; }
        public virtual DbSet<BusinessEntityContact> BusinessEntityContacts { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<CountryRegion> CountryRegions { get; set; }
        public virtual DbSet<EmailAddress> EmailAddresses { get; set; }
        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PersonPhone> PersonPhones { get; set; }
        public virtual DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public virtual DbSet<StateProvince> StateProvinces { get; set; }
        public virtual DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public virtual DbSet<Culture> Cultures { get; set; }
        public virtual DbSet<Illustration> Illustrations { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCostHistory> ProductCostHistories { get; set; }
        public virtual DbSet<ProductDescription> ProductDescriptions { get; set; }
        public virtual DbSet<ProductInventory> ProductInventories { get; set; }
        public virtual DbSet<ProductListPriceHistory> ProductListPriceHistories { get; set; }
        public virtual DbSet<ProductModel> ProductModels { get; set; }
        public virtual DbSet<ProductModelIllustration> ProductModelIllustrations { get; set; }
        public virtual DbSet<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures { get; set; }
        public virtual DbSet<ProductPhoto> ProductPhotoes { get; set; }
        public virtual DbSet<ProductProductPhoto> ProductProductPhotoes { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<ProductSubcategory> ProductSubcategories { get; set; }
        public virtual DbSet<ScrapReason> ScrapReasons { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }
        public virtual DbSet<TransactionHistoryArchive> TransactionHistoryArchives { get; set; }
        public virtual DbSet<UnitMeasure> UnitMeasures { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }
        public virtual DbSet<WorkOrderRouting> WorkOrderRoutings { get; set; }
        public virtual DbSet<ProductVendor> ProductVendors { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual DbSet<ShipMethod> ShipMethods { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<CountryRegionCurrency> CountryRegionCurrencies { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyRate> CurrencyRates { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<PersonCreditCard> PersonCreditCards { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual DbSet<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons { get; set; }
        public virtual DbSet<SalesPerson> SalesPersons { get; set; }
        public virtual DbSet<SalesPersonQuotaHistory> SalesPersonQuotaHistories { get; set; }
        public virtual DbSet<SalesReason> SalesReasons { get; set; }
        public virtual DbSet<SalesTaxRate> SalesTaxRates { get; set; }
        public virtual DbSet<SalesTerritory> SalesTerritories { get; set; }
        public virtual DbSet<SalesTerritoryHistory> SalesTerritoryHistories { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual DbSet<SpecialOffer> SpecialOffers { get; set; }
        public virtual DbSet<SpecialOfferProduct> SpecialOfferProducts { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<ProductDocument> ProductDocuments { get; set; }

        // Views

        //public virtual DbSet<vEmployee> vEmployees { get; set; }
        public virtual DbSet<vEmployeeDepartment> vEmployeeDepartments { get; set; }
        //public virtual DbSet<vEmployeeDepartmentHistory> vEmployeeDepartmentHistories { get; set; }
        //public virtual DbSet<vJobCandidate> vJobCandidates { get; set; }
        //public virtual DbSet<vJobCandidateEducation> vJobCandidateEducations { get; set; }
        //public virtual DbSet<vJobCandidateEmployment> vJobCandidateEmployments { get; set; }
        //public virtual DbSet<vAdditionalContactInfo> vAdditionalContactInfoes { get; set; }
        //public virtual DbSet<vStateProvinceCountryRegion> vStateProvinceCountryRegions { get; set; }
        //public virtual DbSet<vProductAndDescription> vProductAndDescriptions { get; set; }
        //public virtual DbSet<vProductModelCatalogDescription> vProductModelCatalogDescriptions { get; set; }
        //public virtual DbSet<vProductModelInstruction> vProductModelInstructions { get; set; }
        //public virtual DbSet<vVendorWithAddress> vVendorWithAddresses { get; set; }
        //public virtual DbSet<vVendorWithContact> vVendorWithContacts { get; set; }
        //public virtual DbSet<vIndividualCustomer> vIndividualCustomers { get; set; }
        //public virtual DbSet<vPersonDemographic> vPersonDemographics { get; set; }
        //public virtual DbSet<vSalesPerson> vSalesPersons { get; set; }
        //public virtual DbSet<vSalesPersonSalesByFiscalYear> vSalesPersonSalesByFiscalYears { get; set; }
        //public virtual DbSet<vStoreWithAddress> vStoreWithAddresses { get; set; }
        //public virtual DbSet<vStoreWithContact> vStoreWithContacts { get; set; }
        //public virtual DbSet<vStoreWithDemographic> vStoreWithDemographics { get; set; }

        public virtual DbSet<vUser> vUsers { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EntitiesConfiguration.ConfigureTables(modelBuilder);

            EntitiesConfiguration.ConfigureViews(modelBuilder);

            //MapperConfig.RegisterMaps();
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified || e.State == EntityState.Added) && e.Entity is IAuditable).Select(e => e.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                ((IAuditable)modifiedEntry).LastModifiedBy = UserId;
                ((IAuditable)modifiedEntry).LastModifiedDate = DateTime.Now;
            }

            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e);

                if (e.InnerException == null)
                    throw;

                Exception ex = e;

                if (ex.Message == "An error occurred while updating the entries. See the inner exception for details" && e.InnerException != null)
                    ex = e.InnerException;

                if (ex.Message.StartsWith("The DELETE statement conflicted with the REFERENCE constraint"))
                    throw new Exception("Delete failed: linked data found!");

                throw ex;
            }
        }

        public int SaveChangesWithoutModified()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception e)
            {
                Log.Error(e);

                if (e.InnerException == null)
                    throw;

                Exception ex = e;

                if (ex.Message == "An error occurred while updating the entries. See the inner exception for details" && e.InnerException != null)
                    ex = e.InnerException;

                if (ex.Message.StartsWith("The DELETE statement conflicted with the REFERENCE constraint"))
                    throw new Exception("Delete failed: linked data found!");

                throw ex;
            }
        }
    }
}
