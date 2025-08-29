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
        private readonly BaseRepository<Contact> _contactRepository;
        private readonly BaseRepository<Product> _productRepository;
        private readonly BaseRepository<Transaction> _transactionRepository;
        private readonly BaseRepository<TransactionProductItem> _transactionProductItemRepository;
        private readonly Supabase.Client _supabaseClient;
        private readonly SupabaseAuthService _supabaseAuthService;
        private Guid _currentUserId;
        private Dictionary<int, int> _transactionIdMap = new Dictionary<int, int>();
        #endregion

        #region Constructor
        public SyncService(Supabase.Client supabaseClient, SupabaseAuthService supabaseAuthService,
            BaseRepository<Product> products,
            BaseRepository<Contact> contacts, BaseRepository<Transaction> transactions,
            BaseRepository<TransactionProductItem> transactionItems)
        {
            _supabaseClient = supabaseClient;
            _supabaseAuthService = supabaseAuthService;
            _currentUserId = _supabaseAuthService.GetUserIdGuid();
            _contactRepository = contacts;
            _productRepository = products;
            _transactionRepository = transactions;
            _transactionProductItemRepository = transactionItems;
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
                await SyncTransactionsToSupabase(); // This will populate _transactionIdMap
                await SyncTransactionItemsToSupabase(); // This uses _transactionIdMap
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

                ClearLocalData();

                await SyncContactsFromSupabase();
                await SyncProductsFromSupabase();
                await SyncTransactionsFromSupabase();
                await SyncTransactionItemsFromSupabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during sync from Supabase: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Sync To Supabase Methods
        private async Task SyncContactsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var contacts = _contactRepository.GetItems();
            foreach (var contact in contacts)
            {
                try
                {
                    // Check if contact already exists
                    var existing = await _supabaseClient
                        .From<ContactDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == contact.Id)
                        .Get();

                    var contactDto = ContactDto.FromModel(contact, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        // Update existing record
                        contactDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<ContactDto>().Upsert(contactDto);
                        Debug.WriteLine($"✅ Updated contact local_id {contact.Id} -> db_id {contactDto.Id}");
                    }
                    else
                    {
                        // Insert new record - DON'T set Id, let database auto-generate
                        await _supabaseClient.From<ContactDto>().Insert(contactDto);
                        Debug.WriteLine($"✅ Inserted contact local_id {contact.Id}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error syncing contact local_id {contact.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncProductsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var products = _productRepository.GetItems();
            foreach (var product in products)
            {
                try
                {
                    // Check if product already exists
                    var existing = await _supabaseClient
                        .From<ProductDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == product.Id)
                        .Get();

                    var productDto = ProductDto.FromModel(product, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        // Update existing record
                        productDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<ProductDto>().Upsert(productDto);
                        Debug.WriteLine($"✅ Updated product local_id {product.Id} -> db_id {productDto.Id}");
                    }
                    else
                    {
                        // Insert new record - DON'T set Id, let database auto-generate
                        await _supabaseClient.From<ProductDto>().Insert(productDto);
                        Debug.WriteLine($"✅ Inserted product local_id {product.Id}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error syncing product local_id {product.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncTransactionsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            var transactions = _transactionRepository.GetItemsWithChildren();
            Debug.WriteLine($"Found {transactions.Count()} transactions to sync");

            foreach (var transaction in transactions)
            {
                try
                {
                    // Check if transaction already exists
                    var existing = await _supabaseClient
                        .From<TransactionDto>()
                        .Where(x => x.UserId == _currentUserId && x.LocalId == transaction.Id)
                        .Get();

                    var transactionDto = TransactionDto.FromModel(transaction, _currentUserId.ToString());

                    if (existing.Models.Count > 0)
                    {
                        // Update existing record
                        transactionDto.Id = existing.Models[0].Id;
                        await _supabaseClient.From<TransactionDto>().Upsert(transactionDto);
                        Debug.WriteLine($"✅ Updated transaction local_id {transaction.Id} -> db_id {transactionDto.Id}");
                    }
                    else
                    {
                        // Insert new record - DON'T set Id, let database auto-generate
                        var response = await _supabaseClient.From<TransactionDto>().Insert(transactionDto);
                        Debug.WriteLine($"✅ Inserted transaction local_id {transaction.Id}");
                        if (response.Model?.Id > 0)
                        {
                            Debug.WriteLine($"   Generated db_id: {response.Model.Id}");
                        }
                        else
                        {
                            Debug.WriteLine("   Failed to generate db_id");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error syncing transaction local_id {transaction.Id}: {ex.Message}");
                }
            }

            // Build mapping after all transactions are synced
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

                Debug.WriteLine($"Found {allTransactions.Models.Count} transactions in Supabase");

                foreach (var transactionDto in allTransactions.Models)
                {
                    // Remove the if check to include ALL transactions
                    _transactionIdMap[transactionDto.LocalId] = transactionDto.Id;
                    Debug.WriteLine($"Mapped local_id {transactionDto.LocalId} -> db_id {transactionDto.Id}");
                }
                Debug.WriteLine($"Built mapping with {_transactionIdMap.Count} entries");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error building transaction ID map: {ex.Message}");
            }
        }

        private async Task SyncTransactionItemsToSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            // Ensure we have the latest mapping
            if (_transactionIdMap.Count == 0)
            {
                await BuildTransactionIdMap();
            }

            var transactionItems = _transactionProductItemRepository.GetItems();
            Debug.WriteLine($"Found {transactionItems.Count()} transaction items to sync");

            int processedCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            foreach (var item in transactionItems)
            {
                try
                {
                    // Map the local transaction ID to database transaction ID
                    if (_transactionIdMap.TryGetValue(item.TransactionId, out int databaseTransactionId))
                    {
                        // Create item with mapped database transaction ID
                        var mappedItem = new TransactionProductItem
                        {
                            Name = item.Name,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            Cost = item.Cost,
                            TransactionId = databaseTransactionId // Use database ID, not local ID
                        };

                        var itemDto = TransactionProductItemDto.FromModel(mappedItem, _currentUserId.ToString());

                        // Use OnConflict with the UNIQUE CONSTRAINT COLUMNS
                        var response = await _supabaseClient
                            .From<TransactionProductItemDto>()
                            .OnConflict("user_id,local_id") // This is the unique constraint!
                            .Upsert(itemDto);

                        // Debug the response
                        if (response.Model != null)
                        {
                            Debug.WriteLine($"✅ Synced transaction item local_id {item.Id} -> db_id {response.Model.Id}");
                        }
                        else
                        {
                            Debug.WriteLine($"✅ Synced transaction item local_id {item.Id} - no model returned");
                        }
                        processedCount++;
                    }
                    else
                    {
                        Debug.WriteLine($"⚠️ No mapped database ID for transaction local_id {item.TransactionId}");
                        skippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error syncing transaction item local_id {item.Id}: {ex.Message}");
                    errorCount++;
                }
            }

            Debug.WriteLine($"Transaction items: {processedCount} processed, {skippedCount} skipped, {errorCount} errors");
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
                        _contactRepository.InsertItem(contact);
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
                        _productRepository.InsertItem(product);
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
                var supabaseTransactions = await _supabaseClient
                    .From<TransactionDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                foreach (var transactionDto in supabaseTransactions.Models)
                {
                    try
                    {
                        var transaction = transactionDto.ToModel();
                        _transactionRepository.InsertItem(transaction);
                        Debug.WriteLine($"✅ Inserted transaction from Supabase: local_id {transactionDto.LocalId}");
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

        private async Task SyncTransactionItemsFromSupabase()
        {
            if (_currentUserId == Guid.Empty) return;

            try
            {
                var supabaseTransactionItems = await _supabaseClient
                    .From<TransactionProductItemDto>()
                    .Where(x => x.UserId == _currentUserId)
                    .Get();

                foreach (var itemDto in supabaseTransactionItems.Models)
                {
                    try
                    {
                        var item = itemDto.ToModel();
                        _transactionProductItemRepository.InsertItem(item);
                        Debug.WriteLine($"✅ Inserted transaction item from Supabase: local_id {itemDto.LocalId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"❌ Error inserting transaction item local_id {itemDto.LocalId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error syncing transaction items from Supabase: {ex.Message}");
            }
        }
        #endregion

        #region Private Methods
        private void ClearLocalData()
        {
            try
            {
                var contacts = _contactRepository.GetItems();
                foreach (var contact in contacts)
                {
                    _contactRepository.DeleteItem(contact);
                }

                var products = _productRepository.GetItems();
                foreach (var product in products)
                {
                    _productRepository.DeleteItem(product);
                }

                var transactions = _transactionRepository.GetItems();
                foreach (var transaction in transactions)
                {
                    _transactionRepository.DeleteItem(transaction);
                }

                var transactionItems = _transactionProductItemRepository.GetItems();
                foreach (var item in transactionItems)
                {
                    _transactionProductItemRepository.DeleteItem(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing local  {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}