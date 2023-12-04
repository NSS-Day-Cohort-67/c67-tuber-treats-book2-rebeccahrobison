using System.IO.Compression;
using Microsoft.VisualBasic;
using TuberTreats.Models;
using TuberTreats.Models.DTOs;

List<Customer> customers = new List<Customer>
{
    new Customer {Id = 1, Name = "Cillian", Address = "567 Wallace St."},
    new Customer {Id = 2, Name = "Maggie", Address = "123 Main St."},
    new Customer {Id = 3, Name = "Saoirse", Address = "789 Oak Ave."},
    new Customer {Id = 4, Name = "Maeve", Address = "456 Elm St."},
    new Customer {Id = 5, Name = "Ciaran", Address = "321 Maple Rd."},
};

List<Topping> toppings = new List<Topping>
{
    new Topping {Id = 1, Name = "Butter"},
    new Topping {Id = 2, Name = "Bacon Bits"},
    new Topping {Id = 3, Name = "Sour Cream"},
    new Topping {Id = 4, Name = "Cheddar Cheese"},
    new Topping {Id = 5, Name = "Chili"},
    new Topping {Id = 6, Name = "Chives"},
    new Topping {Id = 7, Name = "Jalapeños"},
};

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver {Id = 1, Name = "Chuck"},
    new TuberDriver {Id = 2, Name = "Reggie"},
    new TuberDriver {Id = 3, Name = "Lisa"},
    new TuberDriver {Id = 4, Name = "Frank"},
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder 
    {
        Id = 1, 
        OrderPlacedOnDate = new DateTime(2023, 12, 3, 12, 11, 11), 
        CustomerId = 1, 
        TuberDriverId = 1, 
        DeliveredOnDate = new DateTime(2023, 12, 3, 13, 1, 23),
    },
    new TuberOrder 
    {
        Id = 2, 
        OrderPlacedOnDate = new DateTime(2023, 12, 3, 12, 45, 30), 
        CustomerId = 2, 
        TuberDriverId = 1, 
        DeliveredOnDate = new DateTime(2023, 12, 3, 13, 35, 45)
    },
    new TuberOrder 
    {
        Id = 3, 
        OrderPlacedOnDate = new DateTime(2023, 12, 3, 14, 20, 15), 
        CustomerId = 3, 
        TuberDriverId = 3, 
        DeliveredOnDate = new DateTime(2023, 12, 3, 15, 10, 30)
    },
    new TuberOrder 
    {
        Id = 4, 
        OrderPlacedOnDate = new DateTime(2023, 12, 4, 7, 20, 15), 
        CustomerId = 4, 
        TuberDriverId = 4, 
        DeliveredOnDate = new DateTime(2023, 12, 4, 9, 10, 30)
    },
    new TuberOrder
    {
        Id = 5,
        OrderPlacedOnDate = new DateTime(2023, 12, 4, 10, 6, 20),
        CustomerId = 5
    }
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping {Id = 1, TuberOrderId = 1, ToppingId = 1},
    new TuberTopping {Id = 2, TuberOrderId = 1, ToppingId = 2},
    new TuberTopping {Id = 3, TuberOrderId = 1, ToppingId = 4},
    new TuberTopping {Id = 4, TuberOrderId = 2, ToppingId = 1},
    new TuberTopping {Id = 5, TuberOrderId = 2, ToppingId = 3},
    new TuberTopping {Id = 6, TuberOrderId = 3, ToppingId = 3},
    new TuberTopping {Id = 7, TuberOrderId = 3, ToppingId = 4},
    new TuberTopping {Id = 8, TuberOrderId = 3, ToppingId = 5},
    new TuberTopping {Id = 9, TuberOrderId = 3, ToppingId = 6},
    new TuberTopping {Id = 10, TuberOrderId = 3, ToppingId = 7},
    new TuberTopping {Id = 11, TuberOrderId = 5, ToppingId = 3}
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

app.MapGet("/tuberorders", () =>
{
    return tuberOrders.Select(to => new TuberOrderDTO
    {
        Id = to.Id,
        OrderPlacedOnDate = to.OrderPlacedOnDate,
        CustomerId = to.CustomerId,
        Customer = customers.FirstOrDefault(c => c.Id == to.CustomerId) == null ? null : new CustomerDTO
        {
            Id = to.CustomerId,
            Name = customers.First(c => c.Id == to.CustomerId).Name,
            Address = customers.First(c => c.Id == to.CustomerId).Address
        },
        TuberDriverId = to.TuberDriverId,
        TuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == to.TuberDriverId) == null ? null : new TuberDriverDTO
        {
            Id = to.TuberDriverId ?? 0,
            Name = tuberDrivers.First(td => td.Id == to.TuberDriverId).Name
        },
        DeliveredOnDate = to.DeliveredOnDate,
    });
});

app.MapGet("/tuberorders/{id}", (int id) => 
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(to => to.Id == id);
    if (tuberOrder == null)
    {
        return Results.NotFound();
    }

    Customer customer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == tuberOrder.TuberDriverId);

    List<TuberTopping> foundTuberToppings = tuberToppings.Where(tt => tt.TuberOrderId == id).ToList();
    List<Topping> foundToppings = foundTuberToppings.Select(tt => toppings.First(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(new TuberOrderDTO
    {
        Id = tuberOrder.Id,
        OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
        CustomerId = tuberOrder.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        TuberDriverId = tuberOrder.TuberDriverId,
        TuberDriver = tuberDriver == null ? null : new TuberDriverDTO
        {
            Id = tuberDriver.Id,
            Name = tuberDriver.Name
        },
        DeliveredOnDate = tuberOrder.DeliveredOnDate,
        Toppings = foundToppings.Select(ft => new ToppingDTO
        {
            Id = ft.Id,
            Name = ft.Name
        }).ToList()
    });
});

app.MapPost("/tuberorders", (TuberOrder tuberOrder) => {
    tuberOrder.Id = tuberOrders.Max(to => to.Id) + 1;
    tuberOrder.OrderPlacedOnDate = DateTime.Now;

    Customer customer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);

    if (customer == null)
    {
        return Results.BadRequest();
    }
    
    // Add order toppings to tuberTopppings
    foreach (Topping topping in tuberOrder.Toppings)
    {
        TuberTopping newTT = new TuberTopping
        {
            TuberOrderId = tuberOrder.Id,
            ToppingId = topping.Id
        };
        newTT.Id = tuberToppings.Count > 0 ? tuberToppings.Max(tt => tt.Id) + 1 : 1;
        tuberToppings.Add(newTT);
    }
    // Add order to tuberOrders
    tuberOrders.Add(tuberOrder);

    return Results.Created($"/tuberorders/{tuberOrder.Id}", new TuberOrderDTO
    {
        Id = tuberOrder.Id,
        CustomerId = tuberOrder.CustomerId,
        OrderPlacedOnDate = DateTime.Now,
    }); 
});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder tuberOrder) => 
{
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(o => o.Id == id);

    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != tuberOrder.Id)
    {
        return Results.BadRequest();
    }

    orderToUpdate.TuberDriverId = tuberOrder.TuberDriverId;

    return Results.NoContent();
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    TuberOrder orderToComplete = tuberOrders.FirstOrDefault(to => to.Id == id);

    orderToComplete.DeliveredOnDate = DateTime.Now;
});

app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new ToppingDTO
    {
        Id = t.Id,
        Name = t.Name
    });
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);
    if(topping == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new ToppingDTO
    {
        Id = topping.Id,
        Name = topping.Name
    });
});

app.MapGet("/tubertoppings", () =>
{
    return tuberToppings.Select(tt => new TuberToppingDTO
    {
        Id = tt.Id,
        TuberOrderId = tt.TuberOrderId,
        ToppingId = tt.ToppingId
    });
});

app.MapPost("/tubertoppings", (TuberTopping tuberTopping) =>
{
    tuberTopping.Id = tuberToppings.Max(tt => tt.Id) + 1;
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(to => to.Id == tuberTopping.TuberOrderId);
    Topping topping = toppings.FirstOrDefault(t => t.Id == tuberTopping.ToppingId);
    if(tuberTopping == null || topping == null)
    {
        return Results.BadRequest();
    }

    tuberToppings.Add(tuberTopping);

    return Results.Created($"/tubertoppings/{tuberTopping.Id}", new TuberToppingDTO
    {
        Id = tuberTopping.Id,
        TuberOrderId = tuberTopping.TuberOrderId,
        ToppingId = tuberTopping.ToppingId
    });
});

app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    TuberTopping tuberToppingToDelete = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if(tuberToppingToDelete == null)
    {
        return Results.NotFound();
    }

    tuberToppings.Remove(tuberToppingToDelete);

    return Results.NoContent();
});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

app.MapPost("/customers", (Customer customer) =>
{
    customer.Id = customers.Max(c => c.Id) + 1;

    customers.Add(customer);
    return Results.Created($"/customers/{customer.Id}", new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customerToDelete = customers.FirstOrDefault(c => c.Id == id);
    if (customerToDelete == null)
    {
        return Results.NotFound();
    }
    customers.Remove(customerToDelete);

    return Results.NoContent();
});

app.MapGet("/tuberdrivers", () =>
{
    return tuberDrivers.Select(td => new TuberDriverDTO
    {
        Id = td.Id,
        Name = td.Name
    });
});

app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);
    if(tuberDriver == null)
    {
        return Results.NotFound();
    }

    List<TuberOrder> foundTuberOrders = tuberOrders.Where(to => to.TuberDriverId == tuberDriver.Id).ToList();

    return Results.Ok(new TuberDriverDTO
    {
        Id = tuberDriver.Id,
        Name = tuberDriver.Name,
        TuberDeliveries = foundTuberOrders.Select(fto => new TuberOrderDTO
        {
            Id = fto.Id,
            CustomerId = fto.CustomerId,
            OrderPlacedOnDate = fto.OrderPlacedOnDate,
            DeliveredOnDate = fto.DeliveredOnDate,
            TuberDriverId = tuberDriver.Id
        }).ToList()
    });
});


app.Run();
//don't touch or move this!
public partial class Program { }