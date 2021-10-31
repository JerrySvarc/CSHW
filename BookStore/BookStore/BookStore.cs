using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BookStore
{

	class Program
	{
		static void Main(string[] args)
		{
			StoreModel model = StoreModel.LoadFrom(Console.In);
			StoreView view = new StoreView();
			StoreController controller = new StoreController();


			if (model == null)
			{
				Console.Out.WriteLine("Data error.");
			}
			else
			{
				controller.RunStore(model, view);
			}

			Console.WriteLine();
		}
	}
	//
	// View
	//
	class StoreView
	{
		public void WriteTable(IList<Book> catalogue)
        {
            IList<Book> books = new List<Book>();
            foreach (var book in catalogue)
            {
                books.Add(book);
            }
            int count = 0;
            Console.Out.WriteLine("	<table>");
			while (books.Count > 0)
			{
				switch (count)
				{
					case 0:
						Console.Out.WriteLine("		<tr>");
						Console.Out.WriteLine("			<td style=\"padding: 10px;\">");
						Console.Out.WriteLine($"				<a href=\"/Books/Detail/{books[0].Id}\">{books[0].Title}</a><br />");
						Console.Out.WriteLine($"				Author: {books[0].Author}<br />");
						Console.Out.WriteLine($"				Price: {books[0].Price} EUR &lt;<a href=\"/ShoppingCart/Add/{books[0].Id}\">Buy</a>&gt;");
						Console.Out.WriteLine("			</td>");
						count++;
                        
						books.RemoveAt(0);
						break;
					case 1:
						Console.Out.WriteLine("			<td style=\"padding: 10px;\">");
						Console.Out.WriteLine($"				<a href=\"/Books/Detail/{books[0].Id}\">{books[0].Title}</a><br />");
						Console.Out.WriteLine($"				Author: {books[0].Author}<br />");
						Console.Out.WriteLine($"				Price: {books[0].Price} EUR &lt;<a href=\"/ShoppingCart/Add/{books[0].Id}\">Buy</a>&gt;");
						Console.Out.WriteLine("			</td>");
						count++;
                        
						books.RemoveAt(0);
						break;
					case 2:
						Console.Out.WriteLine("			<td style=\"padding: 10px;\">");
						Console.Out.WriteLine($"				<a href=\"/Books/Detail/{books[0].Id}\">{books[0].Title}</a><br />");
						Console.Out.WriteLine($"				Author: {books[0].Author}<br />");
						Console.Out.WriteLine($"				Price: {books[0].Price} EUR &lt;<a href=\"/ShoppingCart/Add/{books[0].Id}\">Buy</a>&gt;");
						Console.Out.WriteLine("			</td>");
						
                        books.RemoveAt(0);
                        if (books.Count != 0)
                        {
                            Console.Out.WriteLine("		</tr>");
                            count = 0;
                        }
						break;
				}
			}
            Console.Out.WriteLine("		</tr>");
            Console.Out.WriteLine("	</table>");
		}

		public void WriteError() //done
		{
			Console.Out.WriteLine("<!DOCTYPE html>");
			Console.Out.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			Console.Out.WriteLine("<head>");
			Console.Out.WriteLine("	<meta charset=\"utf-8\" />");
			Console.Out.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
			Console.Out.WriteLine("</head>");
			Console.Out.WriteLine("<body>");
			Console.Out.WriteLine("<p>Invalid request.</p>");
			Console.Out.WriteLine("</body>");
			Console.Out.WriteLine("</html>");
			Console.Out.WriteLine("====");
		}

		public void WriteAllBooks(IList<Book> books, string name, int cartCount)
		{
			Console.Out.WriteLine("<!DOCTYPE html>");
			Console.Out.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			Console.Out.WriteLine("<head>");
			Console.Out.WriteLine("	<meta charset=\"utf-8\" />");
			Console.Out.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
			Console.Out.WriteLine("</head>");
			Console.Out.WriteLine("<body>");
			Console.Out.WriteLine("	<style type=\"text/css\">");
			Console.Out.WriteLine("		table, th, td {");
			Console.Out.WriteLine("			border: 1px solid black;");
			Console.Out.WriteLine("			border-collapse: collapse;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("		table {");
			Console.Out.WriteLine("			margin-bottom: 10px;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("		pre {");
			Console.Out.WriteLine("			line-height: 70%;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("	</style>");
			Console.Out.WriteLine("	<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
			Console.Out.WriteLine($"	{name}, here is your menu:"); // name
			Console.Out.WriteLine("	<table>");
			Console.Out.WriteLine("		<tr>");
			Console.Out.WriteLine("			<td><a href=\"/Books\">Books</a></td>");
			Console.Out.WriteLine($"			<td><a href=\"/ShoppingCart\">Cart ({cartCount})</a></td>");
			Console.Out.WriteLine("		</tr>");
			Console.Out.WriteLine("	</table>");
			Console.Out.WriteLine("	Our books for you:");
			//table with books
			if (books.Count == 0)
			{
				Console.Out.WriteLine("	<table>");
				Console.Out.WriteLine("	</table>");
			}
			else
			{
				WriteTable(books);
			}
			Console.Out.WriteLine("</body>");
			Console.Out.WriteLine("</html>");
			Console.Out.WriteLine("====");

		}

		public void WriteBookDetail(Book book, string name, int cartCount) //done
		{
			Console.Out.WriteLine("<!DOCTYPE html>");
			Console.Out.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			Console.Out.WriteLine("<head>");
			Console.Out.WriteLine("	<meta charset=\"utf-8\" />");
			Console.Out.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
			Console.Out.WriteLine("</head>");
			Console.Out.WriteLine("<body>");
			Console.Out.WriteLine("	<style type=\"text/css\">");
			Console.Out.WriteLine("		table, th, td {");
			Console.Out.WriteLine("			border: 1px solid black;");
			Console.Out.WriteLine("			border-collapse: collapse;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("		table {");
			Console.Out.WriteLine("			margin-bottom: 10px;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("		pre {");
			Console.Out.WriteLine("			line-height: 70%;");
			Console.Out.WriteLine("		}");
			Console.Out.WriteLine("	</style>");
			Console.Out.WriteLine("	<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
			Console.Out.WriteLine($"	{name}, here is your menu:");
			Console.Out.WriteLine("	<table>");
			Console.Out.WriteLine("		<tr>");
			Console.Out.WriteLine("			<td><a href=\"/Books\">Books</a></td>");
			Console.Out.WriteLine($"			<td><a href=\"/ShoppingCart\">Cart ({cartCount})</a></td>");
			Console.Out.WriteLine("		</tr>");
			Console.Out.WriteLine("	</table>");
			Console.Out.WriteLine("	Book details:");
			//detail knihy
			Console.Out.WriteLine($"	<h2>{book.Title}</h2>");
			Console.Out.WriteLine("	<p style=\"margin-left: 20px\">");
			Console.Out.WriteLine($"	Author: {book.Author}<br />");
			Console.Out.WriteLine($"	Price: {book.Price} EUR<br />");
			Console.Out.WriteLine("	</p>");
			Console.Out.WriteLine($"	<h3>&lt;<a href=\"/ShoppingCart/Add/{book.Id}\">Buy this book</a>&gt;</h3>");
			Console.Out.WriteLine("</body>");
			Console.Out.WriteLine("</html>");
			Console.Out.WriteLine("====");
		}

		public void WriteCartItems(int cartCount, StoreModel model, ShoppingCart cart, string name)  //done
		{
			if (cartCount == 0)
			{
				Console.Out.WriteLine("<!DOCTYPE html>");
				Console.Out.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
				Console.Out.WriteLine("<head>");
				Console.Out.WriteLine("	<meta charset=\"utf-8\" />");
				Console.Out.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
				Console.Out.WriteLine("</head>");
				Console.Out.WriteLine("<body>");
				Console.Out.WriteLine("	<style type=\"text/css\">");
				Console.Out.WriteLine("		table, th, td {");
				Console.Out.WriteLine("			border: 1px solid black;");
				Console.Out.WriteLine("			border-collapse: collapse;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("		table {");
				Console.Out.WriteLine("			margin-bottom: 10px;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("		pre {");
				Console.Out.WriteLine("			line-height: 70%;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("	</style>");
				Console.Out.WriteLine("	<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
				Console.Out.WriteLine($"	{name}, here is your menu:");
				Console.Out.WriteLine("	<table>");
				Console.Out.WriteLine("		<tr>");
				Console.Out.WriteLine("			<td><a href=\"/Books\">Books</a></td>");
				Console.Out.WriteLine($"			<td><a href=\"/ShoppingCart\">Cart ({cartCount})</a></td>");
				Console.Out.WriteLine("		</tr>");
				Console.Out.WriteLine("	</table>");
				Console.Out.WriteLine("	Your shopping cart is EMPTY.");
				Console.Out.WriteLine("</body>");
				Console.Out.WriteLine("</html>");
				Console.Out.WriteLine("====");
			}
			else
			{
				Console.Out.WriteLine("<!DOCTYPE html>");
				Console.Out.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
				Console.Out.WriteLine("<head>");
				Console.Out.WriteLine("	<meta charset=\"utf-8\" />");
				Console.Out.WriteLine("	<title>Nezarka.net: Online Shopping for Books</title>");
				Console.Out.WriteLine("</head>");
				Console.Out.WriteLine("<body>");
				Console.Out.WriteLine("	<style type=\"text/css\">");
				Console.Out.WriteLine("		table, th, td {");
				Console.Out.WriteLine("			border: 1px solid black;");
				Console.Out.WriteLine("			border-collapse: collapse;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("		table {");
				Console.Out.WriteLine("			margin-bottom: 10px;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("		pre {");
				Console.Out.WriteLine("			line-height: 70%;");
				Console.Out.WriteLine("		}");
				Console.Out.WriteLine("	</style>");
				Console.Out.WriteLine("	<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
				Console.Out.WriteLine($"	{name}, here is your menu:");
				Console.Out.WriteLine("	<table>");
				Console.Out.WriteLine("		<tr>");
				Console.Out.WriteLine("			<td><a href=\"/Books\">Books</a></td>");
				Console.Out.WriteLine($"			<td><a href=\"/ShoppingCart\">Cart ({cartCount})</a></td>");
				Console.Out.WriteLine("		</tr>");
				Console.Out.WriteLine("	</table>");
				Console.Out.WriteLine("	Your shopping cart:");
				//cart items
				Console.Out.WriteLine("	<table>");
				Console.Out.WriteLine("		<tr>");
				Console.Out.WriteLine("			<th>Title</th>");
				Console.Out.WriteLine("			<th>Count</th>");
				Console.Out.WriteLine("			<th>Price</th>");
				Console.Out.WriteLine("			<th>Actions</th>");
				Console.Out.WriteLine("		</tr>");

                int index = 1;

                decimal price = 0;
				foreach (var item in cart.Items)
				{
					decimal totalPrice = item.Count * model.GetBook(item.BookId).Price;
                    price += totalPrice;
					Console.Out.WriteLine("		<tr>");
					Console.Out.WriteLine($"			<td><a href=\"/Books/Detail/{item.BookId}\">{model.GetBook(item.BookId).Title}</a></td>");
					Console.Out.WriteLine($"			<td>{item.Count}</td>");
                    if (item.Count ==1)
                    {
						Console.Out.WriteLine($"			<td>{model.GetBook(item.BookId).Price} EUR</td>");
					}
                    else
                    {
						Console.Out.WriteLine($"			<td>{item.Count} * {model.GetBook(item.BookId).Price} = {totalPrice} EUR</td>");
					}
                    Console.Out.WriteLine($"			<td>&lt;<a href=\"/ShoppingCart/Remove/{item.BookId}\">Remove</a>&gt;</td>");
					Console.Out.WriteLine("		</tr>");
                    index++;
                }
                Console.Out.WriteLine("	</table>");
				Console.Out.WriteLine($"	Total price of all items: {price} EUR");
                Console.Out.WriteLine("</body>");
				Console.Out.WriteLine("</html>");
				Console.Out.WriteLine("====");
			}
		}

	}

	//
	// Controller
	//
	class StoreController
	{

		public void RunStore(StoreModel model, StoreView view)
		{
			while (true)
			{
				try
				{
					string[] parsedLine = ParseLine(Console.In);
					if (parsedLine.Length == 3 && parsedLine[0] == "GET")
					{
						ParseRequest(parsedLine, model, view);
					}
					else
					{
						view.WriteError();
					}
				}
				catch (Exception e)
				{
					break;
				}
			   
			}
		}


		public string[] ParseLine(TextReader reader)
		{
			string line = reader.ReadLine();
			string[] parsedLine = line.Split(" ");
			return parsedLine;
		}

		public void BookRequest(List<string> parsedRequest, StoreModel model, StoreView view, int customerId)
		{
			
				if (parsedRequest.Count == 3)
				{
					view.WriteAllBooks(model.GetBooks(), model.GetCustomer(customerId).FirstName, model.GetCustomer(customerId).ShoppingCart.Items.Count);
				}
				else if (parsedRequest.Count == 5 && parsedRequest[3] == "Detail" && int.TryParse(parsedRequest[4], out int bookId))
				{
					if (model.GetBook(bookId) != null)
					{
						view.WriteBookDetail(model.GetBook(bookId), model.GetCustomer(customerId).FirstName, model.GetCustomer(customerId).ShoppingCart.Items.Count);
					}
					else
					{
						view.WriteError();
					}
				}
				else
				{
					view.WriteError();
				}
		}

		public void CartRequest(List<string> parsedRequest, StoreModel model, StoreView view, int customerId)
		{
			if (model.GetCustomer(customerId).ShoppingCart != null)
			{
				if (parsedRequest.Count == 3)
				{
					view.WriteCartItems(model.GetCustomer(customerId).ShoppingCart.Items.Count, model, model.GetCustomer(customerId).ShoppingCart, model.GetCustomer(customerId).FirstName);
				}
				else if (parsedRequest.Count == 5 && parsedRequest[3] == "Add" && int.TryParse(parsedRequest[4], out int bookId))
				{
					if (model.GetBook(bookId) != null)
					{
						model.GetCustomer(customerId).AddBook(model.GetBook(bookId));
						view.WriteCartItems(model.GetCustomer(customerId).ShoppingCart.Items.Count, model, model.GetCustomer(customerId).ShoppingCart, model.GetCustomer(customerId).FirstName);
					}
                    else
                    {
                        view.WriteError();
                    }
				}
				else if (parsedRequest.Count == 5 && parsedRequest[3] == "Remove" && int.TryParse(parsedRequest[4], out int bookId2))
				{
					if (model.GetCustomer(customerId).RemoveBook(model.GetBook(bookId2)))
					{
						view.WriteCartItems(model.GetCustomer(customerId).ShoppingCart.Items.Count, model, model.GetCustomer(customerId).ShoppingCart, model.GetCustomer(customerId).FirstName);

					}
					else
					{
						view.WriteError();
					}
				}
				else
				{
					view.WriteError();
				}
			}
			else
			{
				view.WriteError();
			}
		}
		public void ParseRequest(string[] parsedLine, StoreModel model, StoreView view)
		{
			if (int.TryParse(parsedLine[1], out int customerId) && model.GetCustomer(customerId) != null)
			{
				string request = parsedLine[2].Replace('/', ' ');
				string[] parsedRequestNotClean = request.Split(" ");
				List<string> parsedRequest = new List<string>();
				foreach (var word in parsedRequestNotClean)
				{
					if (word != "" && word != " ")
					{
						parsedRequest.Add(word);
					}
				}

				if (parsedRequest.Count >= 3 && parsedRequest[0] == "http:" && parsedRequest[1] == "www.nezarka.net")
				{
					switch (parsedRequest[2])
					{
						case "Books":
							BookRequest(parsedRequest, model, view, customerId);
							break;

						case "ShoppingCart":
							CartRequest(parsedRequest, model, view, customerId);
							break;

						default:
							view.WriteError();
							break;
					}
				}
				else
				{
					view.WriteError();
				}
			}
			else
			{
				view.WriteError();
			}
		}
	}

	//
	// Model
	//
	class StoreModel
	{
		private List<Book> books = new List<Book>();
		private List<Customer> customers = new List<Customer>();

		public IList<Book> GetBooks()
		{
			return books;
		}

		public Book GetBook(int id)
		{
			return books.Find(b => b.Id == id);
		}


		public Customer GetCustomer(int id)
		{
			return customers.Find(c => c.Id == id);
		}

		public static StoreModel LoadFrom(TextReader reader)
		{
			var store = new StoreModel();

			try
			{
				if (reader.ReadLine() != "DATA-BEGIN")
				{
					return null;
				}
				while (true)
				{
					string line = reader.ReadLine();
					if (line == null)
					{
						return null;
					}
					else if (line == "DATA-END")
					{
						break;
					}

					string[] tokens = line.Split(';');
					switch (tokens[0])
					{
						case "BOOK":
							store.books.Add(new Book
							{
								Id = int.Parse(tokens[1]),
								Title = tokens[2],
								Author = tokens[3],
								Price = decimal.Parse(tokens[4])
							});
							break;
						case "CUSTOMER":
							store.customers.Add(new Customer
							{
								Id = int.Parse(tokens[1]),
								FirstName = tokens[2],
								LastName = tokens[3]
							});
							break;
						case "CART-ITEM":
							var customer = store.GetCustomer(int.Parse(tokens[1]));
							if (customer == null)
							{
								return null;
							}
							customer.ShoppingCart.Items.Add(new ShoppingCartItem
							{
								BookId = int.Parse(tokens[2]),
								Count = int.Parse(tokens[3])
							});
							break;
						default:
							return null;
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is IndexOutOfRangeException)
				{
					return null;
				}
				throw;
			}

			return store;
		}
	}

	class Book
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public decimal Price { get; set; }
	}

	class Customer
	{
		private ShoppingCart shoppingCart;

		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public ShoppingCart ShoppingCart
		{
			get
			{
				if (shoppingCart == null)
				{
					shoppingCart = new ShoppingCart();
				}
				return shoppingCart;
			}
			set
			{
				shoppingCart = value;
			}
		}
		public bool RemoveBook(Book book)
		{
            if (book != null)
            {
				ShoppingCartItem bookToRemove = ShoppingCart.Items.Find(c => c.BookId == book.Id);
                if (bookToRemove != null)
                {
                    if (bookToRemove.Count == 1)
                    {
                        ShoppingCart.Items.Remove(bookToRemove);
                    }
                    else
                    {
                        --bookToRemove.Count;
                    }
                    return true;
                }
			}
			return false;
		}

		public void AddBook(Book book)
		{
			ShoppingCartItem cartItem = ShoppingCart.Items.Find(c => c.BookId == book.Id);

			if (cartItem != null)
			{
				cartItem.Count += 1;
			}
			else
			{
				shoppingCart.Items.Add(new ShoppingCartItem { BookId = book.Id, Count = 1 });
			}
		}
	}

	class ShoppingCartItem
	{
		public int BookId { get; set; }
		public int Count { get; set; }
	}

	class ShoppingCart
	{
		public int CustomerId { get; set; }
		public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();
	}


}
