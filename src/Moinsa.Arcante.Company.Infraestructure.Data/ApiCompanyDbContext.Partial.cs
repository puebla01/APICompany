using API.Company.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace API.Company.Infraestructure.Data
{
    public partial class ApiCompanyDbContext : BaseDbContext
    {
        private readonly ILogger<ApiCompanyDbContext> _logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMemoryCache memoryCache;
        private readonly DbContextOptions<ApiCompanyDbContext> options;
        private bool _activeTransaction = false;
        protected readonly IConfiguration _configuration = null;

        public static string SelectedDatabase { get; set; }
        private string _key = "6v9y$B&E)H@MbQeThWmZq4t7w!z%C*F-";



        public ApiCompanyDbContext(DbContextOptions<ApiCompanyDbContext> options, ILoggerFactory loggerFactory,
         IMemoryCache memoryCache, IStringLocalizer<ApiCompanyDbContext> localizer,
          ILogger<ApiCompanyDbContext> logger) : base(options)
        {
            this.options = options;
            this.loggerFactory = loggerFactory;
            this.memoryCache = memoryCache;
            _logger = logger;
        }
        protected internal ILogger<ApiCompanyDbContext> Logger => _logger;
        private string GetCatalogFromConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
        public ApiCompanyDbContext(string connectionString, IConfiguration configuration): base(GetDbContextOptions(connectionString), configuration)
        {
            SelectedDatabase = GetCatalogFromConnectionString(connectionString);
        }
        public ApiCompanyDbContext(string connectionString) : base(GetDbContextOptions(connectionString))
        {
            SelectedDatabase = GetCatalogFromConnectionString(connectionString);
        }
        private static DbContextOptions<ApiCompanyDbContext> GetDbContextOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiCompanyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
        public InfoTransaction BeginTransaction(string nameTransactionPoint = "", [CallerFilePath] string nameClass = "", [CallerMemberName] string nameMethod = "")
        {
            return BeginTransactionAsync(nameTransactionPoint, nameClass, nameMethod).Result;
        }

        public async Task<InfoTransaction> BeginTransactionAsync(string nameTransactionPoint = "", [CallerFilePath] string nameClass = "", [CallerMemberName] string nameMethod = "")
        {
            InfoTransaction infoTransaction = new InfoTransaction { IsNewTransaction = false };
            this.Logger.LogDebug("BeginTransactionAsync init");
            try
            {
                if (_activeTransaction == false)
                {
                    this.Logger.LogDebug("BeginTransactionAsync _activeTransaction = false");
                    await this.Database.BeginTransactionAsync().ConfigureAwait(false);
                    infoTransaction.IsNewTransaction = true;
                    infoTransaction.NameMethodOrigin = nameMethod;
                    this.Logger.LogDebug($"BeginTransactionAsync nameMethod = {nameMethod}");
                    infoTransaction.NameClassOrigin = nameClass;
                    this.Logger.LogDebug($"BeginTransactionAsync nameClass = {nameClass}");
                    _activeTransaction = true;
                }

                if (!string.IsNullOrWhiteSpace(nameTransactionPoint))
                {
                    this.Logger.LogDebug($"BeginTransactionAsync nameTransactionPoint = {nameTransactionPoint}");
                    await Database.ExecuteSqlRawAsync($"save transaction {FormatNameTransactionPoint(nameTransactionPoint)}").ConfigureAwait(false);
                }

                return infoTransaction;
            }
            finally
            {
                this.Logger.LogDebug($"BeginTransactionAsync end");
            }
        }

        private static string FormatNameTransactionPoint(string nameTransactionPoint)
        {
            if (nameTransactionPoint.Length > 32)
            {
                nameTransactionPoint = nameTransactionPoint.Substring(nameTransactionPoint.Length - 32);
            }

            return nameTransactionPoint;
        }
        public void CommitTransaction(InfoTransaction infoTransaction)
        {
            var task = Task.Run(async () => { await CommitTransactionAsync(infoTransaction).ConfigureAwait(false); });
            task.Wait();
        }

        public async Task CommitTransactionAsync(InfoTransaction infoTransaction)
        {
            this.Logger.LogDebug("CommitTransactionAsync init");
            if (_activeTransaction)
            {
                this.Logger.LogDebug($"CommitTransactionAsync _activeTransaction = {_activeTransaction}");
                if (infoTransaction != null)
                {
                    this.Logger.LogDebug("CommitTransactionAsync infoTransaction");
                    if (infoTransaction.IsNewTransaction)
                    {
                        this.Logger.LogDebug("CommitTransactionAsync infoTransaction.isNewTransaction = true");
                        _activeTransaction = false;
                        await Database.CurrentTransaction.CommitAsync().ConfigureAwait(false);
                    }
                }
            }
            this.Logger.LogDebug("CommitTransactionAsync end");
        }

        public void RollbackTransaction(InfoTransaction infoTransaction, string nameTransactionPoint = "")
        {
            var task = Task.Run(async () => { await RollbackTransactionAsync(infoTransaction, nameTransactionPoint).ConfigureAwait(false); });
            task.Wait();
        }

        public async Task RollbackTransactionAsync(InfoTransaction infoTransaction, string nameTransactionPoint = "")
        {
            this.Logger.LogDebug("RollbackTransactionAsync init");
            if (_activeTransaction)
            {
                this.Logger.LogDebug($"RollbackTransactionAsync _activeTransaction = {_activeTransaction}");
                if (!string.IsNullOrWhiteSpace(nameTransactionPoint))
                {
                    this.Logger.LogDebug($"RollbackTransactionAsync nameTransactionPoint = {nameTransactionPoint}");
                    await Database.ExecuteSqlRawAsync($"rollback transaction {FormatNameTransactionPoint(nameTransactionPoint)}").ConfigureAwait(false);
                }

                if (infoTransaction != null)
                {
                    this.Logger.LogDebug("RollbackTransactionAsync infoTransaction");
                    if (infoTransaction.IsNewTransaction)
                    {
                        this.Logger.LogDebug("RollbackTransactionAsync infoTransaction.isNewTransaction = true");
                        _activeTransaction = false;
                        await Database.CurrentTransaction.RollbackAsync().ConfigureAwait(false);
                    }
                }
            }
            this.Logger.LogDebug("RollbackTransactionAsync end");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger?.LogDebug($"{nameof(ApiCompanyDbContext)}.{nameof(OnConfiguring)} Start");

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif

            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Data Source=172.26.19.9\\SQL2017;Initial Catalog=ARCANTE_API;Persist Security Info=True;User ID=sa;Password=Nirvana49;trustservercertificate=true");
                optionsBuilder.UseSqlServer("Data Source=localhost\\MSSQLEXPRESS;Initial Catalog=Company;Persist Security Info=True;User ID=sa;Password=001001;MultipleActiveResultSets=True;trustservercertificate=true");
                _key = "This value must be replaced in config file";
            }

            if (_configuration != null && !string.IsNullOrWhiteSpace(_configuration["Application:EncryptionKey"]))
            {
                _key = _configuration["Application:EncryptionKey"];
            }

            if (string.IsNullOrEmpty(_key))
            {
                _logger?.LogCritical($"{nameof(ApiCompanyDbContext)}.{nameof(OnConfiguring)} falta el parámetro Application:EncryptionKey en el fichero de configuración");
            }

            _logger?.LogDebug($"{nameof(ApiCompanyDbContext)}.{nameof(OnConfiguring)} End");
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Infraestructure.Data.Configurations.OrganizationsConfiguration());


            modelBuilder
               .Entity<Organizations>()
               .Property(o => o.CadenaConexion)
               .HasColumnType("varchar(MAX)")
               .HasConversion(
                   x => EncryptString(x, _key),
                   x => DecryptString(x, _key)
               );

            base.OnModelCreating(modelBuilder);
        }
        private static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            //var cipher = new byte[16];
            var cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            //Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            //Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        public static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }
    }
}