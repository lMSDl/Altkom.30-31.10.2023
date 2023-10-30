﻿

using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

var contextOptions = new DbContextOptionsBuilder<Context>()
                        .UseSqlServer(@"Server=(local)\SQLEXPRESS;Database=EFCore;Integrated Security=true;TrustServerCertificate=True") //Encrypt=True
                        //.LogTo(Console.WriteLine)
                        .Options;


var order = new Order() { };
var product = new Product() { Name = "Marchewka", Price = 15 };

using (var context = new Context(contextOptions))
{
    //wyłączenie automatycznego wykrywania zmian
    //context.ChangeTracker.AutoDetectChangesEnabled = false;
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();


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
}

using (var context = new Context(contextOptions))
{
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

}