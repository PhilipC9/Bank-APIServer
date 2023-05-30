using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

WebApplication app = WebApplication.Create();

app.Urls.Add("http://localhost:3000");
app.Urls.Add("http://*:3000");


Bank bank = new Bank();
bank.Namn = "Sparkonto";
bank.Pengar = 1000;
bank.Ränta = 4;


// GET som hämtar data från banken och HTML design
app.MapGet("/", (HttpContext context) =>
{
    double ränta = bank.Ränta / 100.0;
    int avkastning = (int)(bank.Pengar * ränta);
    

    // HTML för hemsidan
    string pageContent = $@"
        <html>
            <head>
                <meta charset='UTF-8'>
            </head>
            <body>
                <h1>Konto Typ: {bank.Namn}</h1>
                <p>Saldo: {bank.Pengar} kr</p>
                <p>Ränta: {bank.Ränta}%</p>
                <p>Avkastning med ränta: {avkastning} kr</p>
                
                <form method='post' action='/deposit'>
                    <label for='depositAmount'>Insättning:</label>
                    <input type='number' id='depositAmount' name='depositAmount'>
                    <button type='submit'>Insättning</button>
                </form>
                
                <form method='post' action='/withdraw'>
                    <label for='withdrawAmount'>Uttag:</label>
                    <input type='number' id='withdrawAmount' name='withdrawAmount'>
                    <button type='submit'>Uttag</button>
                </form>
            </body>
        </html>
    ";

    context.Response.ContentType = "text/html; charset=UTF-8";
    return context.Response.WriteAsync(pageContent);
});


// Post för att lägga in pengar, kan kallas med knapp och input eller med /deposit i url
app.MapPost("/deposit", (HttpContext context) =>
{
    int depositAmount = int.Parse(context.Request.Form["depositAmount"]);
    bank.Pengar += depositAmount;

    context.Response.Redirect("/");
    return Task.CompletedTask;
});


// POST För att ta ut pengar kan kallas med knapp och input eller med /withdraw i url
app.MapPost("/withdraw", (HttpContext context) =>
{
    int withdrawAmount = int.Parse(context.Request.Form["withdrawAmount"]);
    if (withdrawAmount <= bank.Pengar)
    {
        bank.Pengar -= withdrawAmount;
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return Task.CompletedTask;
    }

    context.Response.Redirect("/");
    return Task.CompletedTask;
});

app.Run();
