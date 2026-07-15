using System;
using System.IO;
using System.Linq;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

bool running = true;
string[] itemNames = new string[5];
double[] itemPrices = new double[5];
int[] itemQtys = new int[5];
int currCount = 0;
double gstPercent = 5;
double tipAmount = 0;

while (running)
{
    Console.Write(GetMenu());
    string input = Console.ReadLine();
    if (int.TryParse(input, out int choice))
    {
        Console.WriteLine();
        switch (choice)
        {
            case 1:
                AddItem();
                break;
            case 0:
                Console.WriteLine("Thanks! Have a nice day!");
                running = false;
                break;
            default:
                Console.WriteLine("Unknown option. Try again. \n");
                break;
        } 
    }
    else
        Console.WriteLine("\nInvalid selection. Please enter a valid menu number.\n");
}

bool IsBillEmpty(int count, string errMessage)
{
    if (count == 0)
    {
        Console.WriteLine($"{errMessage}\n");
        return true;
    }
    return false;
}

bool NameValid(string name) =>
    !string.IsNullOrWhiteSpace(name) && name.Length >= 3 && name.Length <= 20 && name.All(c => char.IsLetterOrDigit(c) || c == ' ');

bool IsPriceValid(double price) => price > 0;
bool IsQtyValid(int qty) => qty >= 1 && qty <= 5;

int Diap(string mess, int min, int max)
{
    while (true)
    {
        string input = Message(mess);
        if (int.TryParse(input, out int choice))
        {
            if (choice >= min && choice <= max) return choice;
        }
        Console.WriteLine($"Wrong choice! Please enter a number between {min} and {max}.\n");
    }
}

double CalculateSub(double[] prices, int[] qtys, int count)
{
    double sub = 0;
    for (int i = 0; i < count; i++)
    {
        sub += qtys[i] * prices[i];
    }
    return sub;
}

double CalculateTotal(double subtotal, double tips, double gst)
{
    double gstCurr = subtotal * (gst / 100);
    return subtotal + tips + gstCurr;
}
bool AddItemToList(string name, double price, int qty)
{
    if (currCount >= 5) return false;
    itemNames[currCount] = name;
    itemPrices[currCount] = price;
    itemQtys[currCount] = qty;
    currCount++;
    return true;
}

void AddItem()
{
    if (currCount >= 5)
    {
        Console.WriteLine("Error: Bill can take only 5 items.\n");
        return;
    }
    string item = "";
    while (true)
    {
        item = Message("Enter description: ");
        if (NameValid(item)) break;
        Console.WriteLine("Wrong input. Item should contain 3-20 symbols and cannot be empty.");
    }
    double price = 0;
    while (true)
    {
        string priceInp = Message("Enter price: ");
        if (double.TryParse(priceInp, NumberStyles.Any, CultureInfo.InvariantCulture, out price) && IsPriceValid(price)) break;
        Console.WriteLine("Wrong input. Price should be positive number.");
    }

    int qty = 0;
    while (true)
    {
        string qtyInp = Message("Enter quantity: ");
        if (int.TryParse(qtyInp, out qty) && IsQtyValid(qty)) break;
        Console.WriteLine("Wrong input. Qty should be integer between 1 and 5");
    }

    AddItemToList(item, price, qty);
    Console.WriteLine("Add item was successful.\n");
}

static string GetMenu()
{
    return
        " _____________________________\n" +
        "|                             |\n" +
        "| | My Cafe                 | |\n" +
        "| | ----------------------- | |\n" +
        "| | 1. Add Item             | |\n" +
        "| | 2. Remove Item          | |\n" +
        "| | 3. Add Tip              | |\n" +
        "| | 4. Display Bill         | |\n" +
        "| | 5. Clear All            | |\n" +
        "| | 6. Save to file         | |\n" +
        "| | 7. Load from file       | |\n" +
        "| | 0. Exit                 | |\n" +
        "| |_________________________| |\n" +
        "|_____________________________|\n" +
        "Enter your choice: ";
}

string Message(string message)
{
    Console.Write(message);
    return Console.ReadLine() ?? "";
}