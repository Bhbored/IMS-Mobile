using IMS_Mobile.DB;
using IMS_Mobile.DTOs;
using IMS_Mobile.MVVM.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile.Service
{
    public class SyncService
    {
        #region Fields
        // Using App's static repositories to ensure we always have fresh instances
        private readonly Supabase.Client _supabaseClient;
        private readonly SupabaseAuthService _supabaseAuthService;
        private Guid _currentUserId;
        private Dictionary<int, int> _transactionIdMap = new Dictionary<int, int>();
        #endregion

        #region Constructor
        public SyncService(Supabase.Client supabaseClient, SupabaseAuthService supabaseAuthService)
        {
            _supabaseClient = supabaseClient;
            _supabaseAuthService = supabaseAuthService;
            _currentUserId = _supabaseAuthService.GetUserIdGuid();
        }
        #endregion

        #region Public Methods
        public void RefreshUserId()
        {
            _currentUserId = _supabaseAuthService.GetUserIdGuid();
        }

        public async Task SyncToSupabase()
        {
            try
            {
                RefreshUserId();

                if (_currentUserId == Guid.Empty)
                {
                    throw new InvalidOperationException("No valid user session.");
                }

                _transactionIdMap.Clear();

                await SyncContactsToSupabase();
                await SyncProductsToSupabase();
                await SyncTransactionsToSupabase();
                await SyncTransactionItemsToSupabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during sync to Supabase: {ex.Message}");
                throw;
            }
        }

        public async Task SyncFromSupabase()
        {
            try
            {
                RefreshUserId();

                if (_currentUserId == Guid.Empty)
                {
                    throw new InvalidOperationException("No valid user session. Please login again.");
                }
                await SyncContactsFromSupabase();
                await SyncProductsFromSupabase();
                await SyncTransactionsFromSupabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during sync from Supabase: {ex.Message}");
                throw;
            }
        }
        public async Task ClearLocalData()
        {
            try
            {
                await Task.Delay(100);
                var contacts = App.ContactRepository?.GetItems() ?? new List<Contact>();
                foreach (var contact in contacts)
                {
                    App.ContactRepository?.DeleteItem(contact);
                }
                await Task.Delay(100);
                var products = App.ProductRepository?.GetItems() ?? new List<Product>();
                foreach (var product in products)
                {
                    App.ProductRepository?.DeleteItem(product);
                }
                await Task.Delay(100);
                var transactions = App.TransactionRepository?.GetItemsWithChildren() ?? new List<Transaction>();
                foreach (var transaction in transactions)
                {
                    App.TransactionRepository?.DeleteItem(transaction);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing local  {ex.Message}");
                throw;
            }
        }
        public async Task ManualSyncToSupabase()
        {
            try
            {
                RefreshUserId();

                if (_currentUserId == Guid.Empty)
                {
                    throw new InvalidOperationException("No valid user session.");
                }

                _transactionIdMap.Clear();
                var task = new List<Task>
                {
                MirrorContactsToSupabase(),
                MirrorProductsToSupabase(),
                MirrorTransactionsToSupabase(),
                };
                await Task.WhenAll(task);
                await MirrorTransactionItemsToSupabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during manual sync to Supabase: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Sync To Supabase Methods
        private async Task SyncContactsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var contacts = App.ContactRepository?.GetItems() ?? new List<Contact>();
            foreach (var contact in contacts)
            {
                try
                {
                    var existing = await _supabaseClient
                        .From<ContactDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == contact.Id)
                        .Get();

                    var contactDto = ContactDto.FromModel(contact, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        contactDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<ContactDto>().Upsert(contactDto);
                    }
                    else
                    {
                        await _supabaseClient.From<ContactDto>().Insert(contactDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing contact {contact.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncProductsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var products = App.ProductRepository?.GetItems() ?? new List<Product>();
            foreach (var product in products)
            {
                try
                {
                    var existing = await _supabaseClient
                        .From<ProductDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == product.Id)
                        .Get();

                    var productDto = ProductDto.FromModel(product, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        productDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<ProductDto>().Upsert(productDto);
                    }
                    else
                    {
                        await _supabaseClient.From<ProductDto>().Insert(productDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing product {product.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncTransactionsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var transactions = App.TransactionRepository?.GetItemsWithChildren() ?? new List<Transaction>();

            foreach (var transaction in transactions)
            {
                try
                {
                    var existing = await _supabaseClient
                        .From<TransactionDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == transaction.Id)
                        .Get();

                    var transactionDto = TransactionDto.FromModel(transaction, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        transactionDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<TransactionDto>().Upsert(transactionDto);
                    }
                    else
                    {
                        await _supabaseClient.From<TransactionDto>().Insert(transactionDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing transaction {transaction.Id}: {ex.Message}");
                }
            }

            await BuildTransactionIdMap();
        }

        private async Task BuildTransactionIdMap()
        {
            _transactionIdMap.Clear();
            try
            {
                var allTransactions = await _supabaseClient
                    .From<TransactionDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                foreach (var transactionDto in allTransactions.Models)
                {
                    _transactionIdMap[transactionDto.LocalId] = transactionDto.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building transaction ID map: {ex.Message}");
            }
        }

        private async Task SyncTransactionItemsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var transactionItems = App.TransactionProductItemRepository?.GetItems() ?? new List<TransactionProductItem>();

            int insertedCount = 0;
            int updatedCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            foreach (var item in transactionItems)
            {
                try
                {
                    var existing = await _supabaseClient
                        .From<TransactionProductItemDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == item.Id)
                        .Get();

                    var itemDto = TransactionProductItemDto.FromModel(item, _currentUserId.ToString());

                    if (_transactionIdMap.TryGetValue(item.TransactionId, out int databaseTransactionId))
                    {
                        itemDto.TransactionId = databaseTransactionId;

                        if (existing.Models.Count > 0)
                        {
                            itemDto.Id = existing.Models[0].Id;
                            await _supabaseClient.From<TransactionProductItemDto>().Upsert(itemDto);
                            updatedCount++;
                        }
                        else
                        {
                            await _supabaseClient.From<TransactionProductItemDto>().Insert(itemDto);
                            insertedCount++;
                        }
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing transaction item {item.Id}: {ex.Message}");
                    errorCount++;
                }
            }
        }
        #endregion

        #region Sync From Supabase Methods
        private async Task SyncContactsFromSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            try
            {
                var supabaseContacts = await _supabaseClient
                    .From<ContactDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                foreach (var contactDto in supabaseContacts.Models)
                {
                    try
                    {
                        var contact = contactDto.ToModel();
                        App.ContactRepository?.InsertItem(contact);
                        Debug.WriteLine($"✅ Inserted contact from Supabase: local_id {contactDto.LocalId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Error inserting contact local_id {contactDto.LocalId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error syncing contacts from Supabase: {ex.Message}");
            }
        }

        private async Task SyncProductsFromSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            try
            {
                var supabaseProducts = await _supabaseClient
                    .From<ProductDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                foreach (var productDto in supabaseProducts.Models)
                {
                    try
                    {
                        var product = productDto.ToModel();
                        App.ProductRepository?.InsertItem(product);
                        Debug.WriteLine($"✅ Inserted product from Supabase: local_id {productDto.LocalId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Error inserting product local_id {productDto.LocalId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error syncing products from Supabase: {ex.Message}");
            }
        }

        private async Task SyncTransactionsFromSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            try
            {
                // Get all transactions
                var supabaseTransactions = await _supabaseClient
                    .From<TransactionDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                // Get all transaction items
                var supabaseTransactionItems = await _supabaseClient
                    .From<TransactionProductItemDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                // Group items by transaction ID for easy lookup
                var itemsByTransactionId = supabaseTransactionItems.Models
                    .GroupBy(x => x.LocalTransactionId)
                    .ToDictionary(g => g.Key, g => g.Select(itemDto => itemDto.ToModel()).ToList());

                // Insert each transaction with its children
                foreach (var transactionDto in supabaseTransactions.Models)
                {
                    try
                    {
                        var transaction = transactionDto.ToModel();

                        // Add the related transaction items (Products)
                        if (itemsByTransactionId.TryGetValue(transaction.Id, out var relatedItems))
                        {
                            transaction.Products = relatedItems; // ✅ This is your Products list
                        }

                        // Insert transaction with all its Products
                        App.TransactionRepository?.InsertItemWithChildren(transaction);
                        Debug.WriteLine($"✅ Inserted transaction with {transaction.Products.Count} products from Supabase: local_id {transactionDto.LocalId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Error inserting transaction local_id {transactionDto.LocalId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error syncing transactions from Supabase: {ex.Message}");
            }
        }


        #endregion

        #region Manual Sync (Mirror) Methods
        private async Task MirrorContactsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var localContacts = (App.ContactRepository?.GetItems() ?? new List<Contact>()).ToDictionary(c => c.Id);
            var supabaseContacts = await _supabaseClient
                .From<ContactDto>()
                .Where(x => x.UserId == _currentUserId)
                .Get();

            var supabaseContactMap = supabaseContacts.Models.ToDictionary(c => c.LocalId);

            foreach (var localContact in localContacts.Values)
            {
                try
                {
                    var contactDto = ContactDto.FromModel(localContact, _currentUserId.ToString());

                    if (supabaseContactMap.TryGetValue(localContact.Id, out var existingDto))
                    {
                        contactDto.Id = existingDto.Id;
                        await _supabaseClient.From<ContactDto>().Upsert(contactDto);
                    }
                    else
                    {
                        await _supabaseClient.From<ContactDto>().Insert(contactDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error mirroring contact {localContact.Id}: {ex.Message}");
                }
            }

            foreach (var supabaseContact in supabaseContacts.Models)
            {
                if (!localContacts.ContainsKey(supabaseContact.LocalId))
                {
                    try
                    {
                        await _supabaseClient.From<ContactDto>().Where(x => x.Id == supabaseContact.Id).Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting contact {supabaseContact.Id}: {ex.Message}");
                    }
                }
            }
        }

        private async Task MirrorProductsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var localProducts = (App.ProductRepository?.GetItems() ?? new List<Product>()).ToDictionary(p => p.Id);
            var supabaseProducts = await _supabaseClient
                .From<ProductDto>()
                .Where(x => x.UserId == _currentUserId)
                .Get();

            var supabaseProductMap = supabaseProducts.Models.ToDictionary(p => p.LocalId);

            foreach (var localProduct in localProducts.Values)
            {
                try
                {
                    var productDto = ProductDto.FromModel(localProduct, _currentUserId.ToString());

                    if (supabaseProductMap.TryGetValue(localProduct.Id, out var existingDto))
                    {
                        productDto.Id = existingDto.Id;
                        await _supabaseClient.From<ProductDto>().Upsert(productDto);
                    }
                    else
                    {
                        await _supabaseClient.From<ProductDto>().Insert(productDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error mirroring product {localProduct.Id}: {ex.Message}");
                }
            }

            foreach (var supabaseProduct in supabaseProducts.Models)
            {
                if (!localProducts.ContainsKey(supabaseProduct.LocalId))
                {
                    try
                    {
                        await _supabaseClient.From<ProductDto>().Where(x => x.Id == supabaseProduct.Id).Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting product {supabaseProduct.Id}: {ex.Message}");
                    }
                }
            }
        }

        private async Task MirrorTransactionsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var localTransactions = (App.TransactionRepository?.GetItemsWithChildren() ?? new List<Transaction>()).ToDictionary(t => t.Id);
            var supabaseTransactions = await _supabaseClient
                .From<TransactionDto>()
                .Where(x => x.UserId == _currentUserId)
                .Get();

            var supabaseTransactionMap = supabaseTransactions.Models.ToDictionary(t => t.LocalId);

            foreach (var localTransaction in localTransactions.Values)
            {
                try
                {
                    var transactionDto = TransactionDto.FromModel(localTransaction, _currentUserId.ToString());

                    if (supabaseTransactionMap.TryGetValue(localTransaction.Id, out var existingDto))
                    {
                        transactionDto.Id = existingDto.Id;
                        await _supabaseClient.From<TransactionDto>().Upsert(transactionDto);
                        Debug.WriteLine($"Updated Supabase Transaction{transactionDto.Id} locally {transactionDto.LocalId}");
                    }
                    else
                    {
                        await _supabaseClient.From<TransactionDto>().Insert(transactionDto);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error mirroring transaction {localTransaction.Id}: {ex.Message}");
                }
            }

            foreach (var supabaseTransaction in supabaseTransactions.Models)
            {
                if (!localTransactions.ContainsKey(supabaseTransaction.LocalId))
                {
                    try
                    {
                        await _supabaseClient.From<TransactionDto>().Where(x => x.Id == supabaseTransaction.Id).Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting transaction {supabaseTransaction.Id}: {ex.Message}");
                    }
                }
            }

            await BuildTransactionIdMap();
        }

        private async Task MirrorTransactionItemsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var localItems = (App.TransactionProductItemRepository?.GetItems() ?? new List<TransactionProductItem>()).ToDictionary(i => i.Id);
            var supabaseItems = await _supabaseClient
                .From<TransactionProductItemDto>()
                .Where(x => x.UserId == _currentUserId)
                .Get();

            var supabaseItemMap = supabaseItems.Models.ToDictionary(i => i.LocalId);

            foreach (var localItem in localItems.Values)
            {
                try
                {
                    if (_transactionIdMap.TryGetValue(localItem.TransactionId, out int databaseTransactionId))
                    {
                        var itemDto = TransactionProductItemDto.FromModel(localItem, _currentUserId.ToString());
                        itemDto.TransactionId = databaseTransactionId;

                        if (supabaseItemMap.TryGetValue(localItem.Id, out var existingDto))
                        {
                            itemDto.Id = existingDto.Id;
                            await _supabaseClient.From<TransactionProductItemDto>().Upsert(itemDto);
                        }
                        else
                        {
                            await _supabaseClient.From<TransactionProductItemDto>().Insert(itemDto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error mirroring transaction item {localItem.Id}: {ex.Message}");
                }
            }

            foreach (var supabaseItem in supabaseItems.Models)
            {
                if (!localItems.ContainsKey(supabaseItem.LocalId))
                {
                    try
                    {
                        await _supabaseClient.From<TransactionProductItemDto>().Where(x => x.Id == supabaseItem.Id).Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deleting transaction item {supabaseItem.Id}: {ex.Message}");
                    }
                }
            }
        }
        #endregion
    }
}