

using DAL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


using var connection = new SqlConnection(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated Security=true;TrustServerCertificate=True");//Encrypt=True

var contextOptions = new DbContextOptionsBuilder<Context>()
                        .UseSqlServer(connection)
                        //.UseChangeTrackingProxies()
                        //Włączenie opóźnionego ładowania - wymaga wirtualizacji właściwości referencji
                        //.UseLazyLoadingProxies()
                        .LogTo(Console.WriteLine)
                        .Options;

var orders = CompileQuery(contextOptions);

foreach (var order in orders)
{
    Console.WriteLine(order.Description);
}

Console.ReadLine();

static void ChangeTracker(DbContextOptions<Context> contextOptions)
{
    var order = new Order() { };
    var product = new Product() { Name = "Marchewka", Price = 15 };

    using (var context = new Context(contextOptions))
    {
        //wyłączenie automatycznego wykrywania zmian
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        /*order = context.CreateProxy<Order>() ;
        product = context.CreateProxy<Product>(x => { x.Name = product.Name; x.Price = product.Price; }) ;*/
        order.Products.Add(product);

        Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
        Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(product).State);

        //context.Attach(order);
        context.Add(order);
        Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
        Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(product).State);

        context.SaveChanges();

        Console.WriteLine("Order po zapisie do bazy: " + context.Entry(order).State);
        Console.WriteLine("Product po zapisie do bazy: " + context.Entry(product).State);

        order.DateTime = DateTime.Now;
        order.Name = "alamakota";
        Console.WriteLine("Order po zmianie daty: " + context.Entry(order).State);
        Console.WriteLine("Order DateTime zmodyfikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
        Console.WriteLine("Order Products zmodyfikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);

        context.Remove(product);
        Console.WriteLine("Product po usunięciu: " + context.Entry(product).State);

        context.SaveChanges();
        Console.WriteLine("Order po zapisie: " + context.Entry(order).State);
        Console.WriteLine("Product po zapisie: " + context.Entry(product).State);

        product.Id = 0;
        context.Attach(product);
        Console.WriteLine("Product: " + context.Entry(product).State);
        context.SaveChanges();
    }


    order.DateTime = order.DateTime.AddMinutes(-60);

    using (var context = new Context(contextOptions))
    {
        context.Attach(order);
        Console.WriteLine("Order: " + context.Entry(order).State);
        //context.Entry(order).State = EntityState.Modified;
        context.Entry(order).Property(x => x.DateTime).IsModified = true;
        Console.WriteLine("Order: " + context.Entry(order).State);
        Console.WriteLine("Order DateTime zmodyfikowany? " + context.Entry(order).Property(x => x.DateTime).IsModified);
        Console.WriteLine("Order Name zmodyfikowany? " + context.Entry(order).Property(x => x.Name).IsModified);
        Console.WriteLine("Order Products zmodyfikowany? " + context.Entry(order).Collection(x => x.Products).IsModified);
        context.SaveChanges();
        /*}

        using (var context = new Context(contextOptions))
        {*/
        context.ChangeTracker.Clear(); //wyczyszczenie kontekstu - alternatywa do tworzenia nowego kontekstu

        //AutoDetectChanges działa w przypadku wywołania: SaveChanges, Local, Entry
        //context.ChangeTracker.AutoDetectChangesEnabled = true;


        order = new Order() { };
        product = new Product() { Name = "Kapusta", Price = 10 };
        order.Products.Add(product);
        product = new Product() { Name = "Ogórki", Price = 7 };
        order.Products.Add(product);

        Console.WriteLine("Przed dodaniem do kontekstu: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        context.Add(order);
        Console.WriteLine("Po dodaniu do kontekstu: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        context.SaveChanges();
        Console.WriteLine("Po zapisie: ");
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        order.DateTime = DateTime.Now;
        product.Price = 4.5f;
        Console.WriteLine("Po zmianie: ");
        context.ChangeTracker.DetectChanges(); //ręczne wykrywanie zmian
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        Console.WriteLine(context.ChangeTracker.DebugView.LongView);


        context.ChangeTracker.Clear();

        var products = new List<Product>() {
        new Product() {Name = "P1", Order = new Order {Id = 60, DateTime = DateTime.Now}},
        new Product() {Name = "P2", Order = new Order()}
    };
        //context.AddRange(products);

        foreach (var p in products)
        {

            context.ChangeTracker.TrackGraph(p.Order, entityEntry =>
            {
                if (entityEntry.Entry.IsKeySet)
                {
                    entityEntry.Entry.State = EntityState.Unchanged;
                }
                else
                {
                    entityEntry.Entry.State = EntityState.Added;
                }
            });
            context.ChangeTracker.TrackGraph(p, entityEntry =>
            {
                entityEntry.Entry.State = EntityState.Added;
            });

        }

        Console.WriteLine(context.ChangeTracker.DebugView.LongView);

        //context.SaveChanges();



    }
}

static Context ConcurrencyToken(DbContextOptions<Context> contextOptions)
{
    var context = new Context(contextOptions);

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var order = new Order() { };
    order.DateTime = DateTime.Now;

    context.Add(order);
    context.SaveChanges();


    order.DateTime = DateTime.Now.AddMinutes(100);
    //order.Name = "alamakota";

    context.SaveChanges();

    var product = new Product() { Order = order, Name = "Marchewka", Price = 15 };

    context.Add(product);
    context.SaveChanges();


    product.Price = product.Price * 1.1f;

    var saved = false;
    do
    {
        try
        {
            context.SaveChanges();
            saved = true;
        }
        catch (DbUpdateConcurrencyException e)
        {

            foreach (var entry in e.Entries)
            {

                //wartości jakie chcmy wprowadzić do bazy
                var currentValues = entry.CurrentValues;
                //wartości jakie pobraliśmy z bazy (historyczne)
                var originalValues = entry.OriginalValues;
                //wartości jakie są aktualnie w bazie danych
                var databaseValues = entry.GetDatabaseValues();

                switch (entry.Entity)
                {
                    case Product:

                        var property = currentValues.Properties.Single(x => x.Name == nameof(Product.Price));
                        var currentPrice = (float)currentValues[property];
                        var originalPrice = (float)originalValues[property];
                        var databasePrice = (float)databaseValues[property];

                        currentPrice = databasePrice + (currentPrice - originalPrice);

                        currentValues[property] = currentPrice;

                        break;
                }
                entry.OriginalValues.SetValues(databaseValues);
            }

        }
    } while (!saved);
    return context;
}

static void ShadowProperty_QueryFilters(DbContextOptions<Context> contextOptions)
{
    var context = new Context(contextOptions);

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    for (int i = 0; i < 17; i++)
    {
        var o = new Order() { };
        o.DateTime = DateTime.Now;
        var orderProduct = new Product() { Name = "Marchewka", Price = 15 };
        o.Products.Add(orderProduct);

        context.Add(o);
    }

    context.SaveChanges();

    context.ChangeTracker.Clear();

    var product = context.Set<Product>().Skip(5).First();

    //odczytywanie wartości ShadowProperty
    var orderId = context.Entry(product).Property<int>("OrderId").CurrentValue;
    orderId = context.Set<Product>().Skip(4).Select(x => EF.Property<int>(x, "OrderId")).First();
    orderId = context.Set<Product>().Skip(4).Select(x => x.Order.Id).First();
    Console.WriteLine(orderId);

    context.Entry(product).Property("OrderId").CurrentValue = 1;
    context.SaveChanges();



    context.Entry(product).Property("IsDeleted").CurrentValue = true;
    //product.IsDeleted = true;
    context.SaveChanges();

    context.ChangeTracker.Clear();

    var products = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.ToList();

    context.ChangeTracker.Clear();

    var order = context.Set<Order>().Find(1);
    products = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.Where(x => EF.Property<int>(x, "OrderId") == 1).ToList();


    context.Entry(order).Property("IsDeleted").CurrentValue = true;
    //order.IsDeleted = true;
    context.SaveChanges();

    context.ChangeTracker.Clear();

    product = context.Set<Product>()/*.Where(x => !x.IsDeleted)*/.First();


    product = context.Set<Product>().IgnoreQueryFilters().First();
    context.ChangeTracker.Clear();
}

static void Transactions(DbContextOptions<Context> contextOptions, bool randomFail)
{
    var context = new Context(contextOptions);

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var products = Enumerable.Range(100, 50).Select(x => new Product { Name = $"Product {x}", Price = 1.23f * x }).ToList();
    var orders = Enumerable.Range(0, 5).Select(x => 
    new Order
    {
        Name = "Zamówienie " + context.Database.SqlQuery<int>($"SELECT NEXT VALUE FOR OrderNumber").AsEnumerable().Single(),
        DateTime = DateTime.Now.AddMinutes(-1.23f * x),
        OrderType = (OrderTypes)(x % 3),
        Parameters = (Parameters)(x % 3)
    })
        .ToList();

    context.RandomFail = randomFail;

    using (var transaction = context.Database.BeginTransaction())
    {
        //using var context2 = new Context(contextOptions);
        //context2.Database.UseTransaction(transaction.GetDbTransaction());

        for (int i = 0; i < orders.Count; i++)
        {
            string savepoint = i.ToString();
            try
            {
                transaction.CreateSavepoint(savepoint);

                var subProducts = products.Skip(i * 10).Take(10).ToList();

                foreach (var product in subProducts)
                {
                    context.Add(product);
                    context.SaveChanges();
                }

                var order = orders[i];
                order.Products = subProducts;
                context.Add(order);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                //transaction.Rollback();
                transaction.RollbackToSavepoint(savepoint);
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
            }

            context.ChangeTracker.Clear();
        }


        transaction.Commit();
    }
}

static void References(DbContextOptions<Context> contextOptions)
{
    Transactions(contextOptions, false);

    Product product;

    using (var context = new Context(contextOptions))
    {
        //EagerLoading
        product = context.Set<Product>()./*AsSplitQuery().*/Include(x => x.Order).ThenInclude(x => x.Products).First();
    }

    using (var context = new Context(contextOptions))
    {
        product = context.Set<Product>().First();

        //ExplicitLoading
        context.Entry(product).Reference(x => x.Order).Load();
        context.Entry(product.Order).Collection(x => x.Products).Load();

        //context.Set<Order>().Where(x => x.Id == context.Entry(product).Property<int>("OrderId").CurrentValue).Load();
    }


    using (var context = new Context(contextOptions))
    {
        product = context.Set<Product>().First();
        //LazyLoading
        if (product.Order != null)
            Console.WriteLine("Order != null");
    }
}

static void TemporalTable(DbContextOptions<Context> contextOptions)
{
    var context = new Context(contextOptions);
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var person = new Person { Name = "Ewa" };

    context.Add(person);
    context.SaveChanges();

    Thread.Sleep(1000);

    person.Name = "Ala";
    context.SaveChanges();


    Thread.Sleep(1000);

    person.Name = "Adam";
    context.SaveChanges();


    Thread.Sleep(1000);

    person.Name = "Wojciech";
    context.SaveChanges();

    context.ChangeTracker.Clear();


    var people = context.Set<Person>().ToList();

    people = context.Set<Person>().TemporalAll().ToList();
    var data = context.Set<Person>().TemporalAll().Select(x => new { x, FROM = EF.Property<DateTime>(x, "From"), TO = EF.Property<DateTime>(x, "To") }).ToList();

    //MS SQL: data zapisywana w UTC
    people = context.Set<Person>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-2)).ToList();

    people = context.Set<Person>().TemporalBetween(DateTime.UtcNow.AddSeconds(-4), DateTime.UtcNow.AddSeconds(-2)).ToList();
}

static IEnumerable<Order> CompileQuery(DbContextOptions<Context> contextOptions)
{
    Transactions(contextOptions, false);
    var context = new Context(contextOptions);

    var orders = context.Set<Order>().AsNoTracking().ToList();

    var timer = new Stopwatch();
    timer.Start();
    orders = Context.GetOrdersByDateRange(context, DateTime.Now.AddMinutes(-5), DateTime.Now).ToList();
    timer.Stop();

    Debug.WriteLine(timer.ElapsedTicks);

    timer.Reset();
    timer.Start();
    orders = Context.GetOrdersByDateRange(context, DateTime.Now.AddMinutes(-5), DateTime.Now).ToList();
    timer.Stop();

    Debug.WriteLine(timer.ElapsedTicks);
    return orders;
}