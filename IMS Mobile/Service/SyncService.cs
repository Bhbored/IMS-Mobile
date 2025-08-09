//using IMS_Mobile.DB;
//using IMS_Mobile.MVVM.Models;
//using Supabase;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Contact = IMS_Mobile.MVVM.Models.Contact;
//namespace IMS_Mobile.Service
//{
//    public class SyncService
//    {
//        private readonly BaseRepository<Contact> _contactRepository;
//        private readonly BaseRepository<Product> _productRepository;
//        private readonly BaseRepository<Transaction> _transactionRepository;
//        private readonly BaseRepository<TransactionProductItem> _transactionProductItemRepository;
//        public SyncService()
//        {
//            _contactRepository = new BaseRepository<Contact>();
//            _productRepository = new BaseRepository<Product>();
//            _transactionRepository = new BaseRepository<Transaction>();
//            _transactionProductItemRepository = new BaseRepository<TransactionProductItem>();
//        }
//        public async Task SyncToSupabase()
//        {
//            var contacts = _contactRepository.GetItems();
//            var products = _productRepository.GetItems();
//            var transactions = _transactionRepository.GetItems();
//            var transactionItems = _transactionProductItemRepository.GetItems();
//            var supabaseClient = Supabase.Client.Instance;
//            foreach (var contact in contacts)
//            {
//                await supabaseClient.From("Contact").Insert(contact);
//            }
//            foreach (var product in products)
//            {
//                await supabaseClient.From("Product").Insert(product);
//            }
//            foreach (var transaction in transactions)
//            {
//                await supabaseClient.From("Transaction").Insert(transaction);
//            }
//            foreach (var item in transactionItems)
//            {
//                await supabaseClient.From("TransactionProductItem").Insert(item);
//            }
//        }
//        public async Task SyncFromSupabase()
//        {
//            var supabaseClient = Supabase.Client.Instance;
//            var contacts = await supabaseClient.From("Contact").Get();
//            var products = await supabaseClient.From("Product").Get();
//            var transactions = await supabaseClient.From("Transaction").Get();
//            var transactionItems = await supabaseClient.From("TransactionProductItem").Get();
//            foreach (var contact in _contactRepository.GetItems())
//            {
//                _contactRepository.DeleteItem(contact);
//            }
//            foreach (var product in _productRepository.GetItems())
//            {
//                _productRepository.DeleteItem(product);
//            }
//            foreach (var transaction in _transactionRepository.GetItems())
//            {
//                _transactionRepository.DeleteItem(transaction);
//            }
//            foreach (var item in _transactionProductItemRepository.GetItems())
//            {
//                _transactionProductItemRepository.DeleteItem(item);
//            }
//            _contactRepository.InsertItems(contacts);
//            _productRepository.InsertItems(products);
//            _transactionRepository.InsertItems(transactions);
//            _transactionProductItemRepository.InsertItems(transactionItems);
//        }
//    }
//}