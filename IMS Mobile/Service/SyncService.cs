using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.DTOs;
using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact = IMS_Mobile.MVVM.Models.Contact;
using System.Diagnostics;

namespace IMS_Mobile.Service
{
    public class SyncService
    {
        private readonly BaseRepository<Contact> _contactRepository;
        private readonly BaseRepository<Product> _productRepository;
        private readonly BaseRepository<Transaction> _transactionRepository;
        private readonly BaseRepository<TransactionProductItem> _transactionProductItemRepository;
        private readonly Supabase.Client _supabaseClient;

        public SyncService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            _contactRepository = new BaseRepository<Contact>();
            _productRepository = new BaseRepository<Product>();
            _transactionRepository = new BaseRepository<Transaction>();
            _transactionProductItemRepository = new BaseRepository<TransactionProductItem>();
        }

        public async Task SyncToSupabase()
        {
            try
            {
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

        private async Task SyncContactsToSupabase()
        {
            var contacts = _contactRepository.GetItems();
            foreach (var contact in contacts)
            {
                try
                {
                    var contactDto = ContactDto.FromModel(contact);
                    await _supabaseClient.From<ContactDto>().Upsert(contactDto);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing contact {contact.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncProductsToSupabase()
        {
            var products = _productRepository.GetItems();
            foreach (var product in products)
            {
                try
                {
                    var productDto = ProductDto.FromModel(product);
                    await _supabaseClient.From<ProductDto>().Upsert(productDto);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing product {product.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncTransactionsToSupabase()
        {
            var transactions = _transactionRepository.GetItemsWithChildren();
            foreach (var transaction in transactions)
            {
                try
                {
                    var transactionDto = TransactionDto.FromModel(transaction);
                    await _supabaseClient.From<TransactionDto>().Upsert(transactionDto);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing transaction {transaction.Id}: {ex.Message}");
                }
            }
        }

        private async Task SyncTransactionItemsToSupabase()
        {
            var transactionItems = _transactionProductItemRepository.GetItems();
            foreach (var item in transactionItems)
            {
                try
                {
                    var itemDto = TransactionProductItemDto.FromModel(item);
                    await _supabaseClient.From<TransactionProductItemDto>().Upsert(itemDto);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error syncing transaction item {item.Id}: {ex.Message}");
                }
            }
        }

        public async Task SyncFromSupabase()
        {
            try
            {
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

        private async Task SyncContactsFromSupabase()
        {
            try
            {
                var supabaseContacts = await _supabaseClient.From<ContactDto>().Get();
                foreach (var contactDto in supabaseContacts.Models)
                {
                    try
                    {
                        var contact = contactDto.ToModel();
                        _contactRepository.InsertItem(contact);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error inserting contact {contactDto.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing contacts from Supabase: {ex.Message}");
            }
        }

        private async Task SyncProductsFromSupabase()
        {
            try
            {
                var supabaseProducts = await _supabaseClient.From<ProductDto>().Get();
                foreach (var productDto in supabaseProducts.Models)
                {
                    try
                    {
                        var product = productDto.ToModel();
                        _productRepository.InsertItem(product);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error inserting product {productDto.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing products from Supabase: {ex.Message}");
            }
        }

        private async Task SyncTransactionsFromSupabase()
        {
            try
            {
                var supabaseTransactions = await _supabaseClient.From<TransactionDto>().Get();
                foreach (var transactionDto in supabaseTransactions.Models)
                {
                    try
                    {
                        var transaction = transactionDto.ToModel();
                        _transactionRepository.InsertItem(transaction);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error inserting transaction {transactionDto.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing transactions from Supabase: {ex.Message}");
            }
        }

        private async Task SyncTransactionItemsFromSupabase()
        {
            try
            {
                var supabaseTransactionItems = await _supabaseClient.From<TransactionProductItemDto>().Get();
                foreach (var itemDto in supabaseTransactionItems.Models)
                {
                    try
                    {
                        var item = itemDto.ToModel();
                        _transactionProductItemRepository.InsertItem(item);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error inserting transaction item {itemDto.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing transaction items from Supabase: {ex.Message}");
            }
        }

        private void ClearLocalData()
        {
            try
            {
                // Delete all items from each repository
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
                Debug.WriteLine($"Error clearing local data: {ex.Message}");
                throw;
            }
        }
    }
}