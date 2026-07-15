using System;
using System.IO;
using System.Linq;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

bool running = true;
string[] itemNames = new string[5];
double[] itemPrices = new double[5];
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
            case 2:
                RemoveItem();
                break;
            case 3:
                if (IsBillEmpty(currCount, "[!] The bill is empty! Add items to the bill before setting tips.")) break;

                double currSub = CalculateSub(itemPrices, currCount);
                string tipMenu = $"Net Total: ${currSub:F2}\n" +
                         "1 - Tip Percentage\n" +
                         "2 - Tip Amount\n" +
                         "0 - No Tip\n" +
                         "Enter Tip Method: ";
                tipAmount = AddTip(tipMenu, currSub);
                string tipStatus = (tipAmount > 0) ? $"\nTip successfully updated to ${tipAmount:F2}\n" : "\nTip amount is $0.00\n";
                break;
            case 4:
                string billText = DisplayBill(itemNames, itemPrices, currCount, tipAmount, gstPercent);
                Console.WriteLine(billText);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
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

int Diap(string mess, int min, int max)
{
    while (true)
    {
        string input = Message(mess);
        if (int.TryParse(input, out int choice))
        {
            if (choice >= min && choice <= max || choice == 0) return choice;
        }
        Console.WriteLine($"Wrong choice! Please enter a number between {min} and {max}.\n");
    }
}

double CalculateSub(double[] prices, int count)
{
    double sub = 0;
    for (int i = 0; i < count; i++)
    {
        sub += prices[i];
    }
    return sub;
}

double CalculateTotal(double subtotal, double tips, double gst)
{
    double gstCurr = subtotal * (gst / 100);
    return subtotal + tips + gstCurr;
}
bool AddItemToList(string name, double price)
{
    if (currCount >= 5) return false;
    itemNames[currCount] = name;
    itemPrices[currCount] = price;
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
        if (double.TryParse(priceInp, CultureInfo.InvariantCulture, out price) && IsPriceValid(price)) break;
        Console.WriteLine("Wrong input. Price should be positive number.");
    }

    AddItemToList(item, price);
    Console.WriteLine("Item was successfully added to list.\n");
}

bool TryRemoveItem(int index, ref int count, string[] names, double[] prices)
{
    if (index < 0 || index >= count) return false;
    for (int i = index; i < count - 1; i++)
    {
        names[i] = names[i + 1];
        prices[i] = prices[i + 1];
    }
    names[count - 1] = null;
    prices[count - 1] = 0;

    count--;
    return true;
}

string RemoveList(string[] names, double[] prices, int count)
{
    string listText = $"{"ItemNo",-6} {"Description",-20} {"Price",10}\n" +
                      "------ -------------------- ----------\n";

    for (int i = 0; i < count; i++)
    {
        string priceFormated = "$" + prices[i].ToString("F2");
        listText += $"{i + 1,6} {names[i],-20}{priceFormated,10}\n";
    }
    return listText;
}


void RemoveItem()
{
    if (IsBillEmpty(currCount, "[!] Bill is empty. There is nothing to remove.")) return;

    Console.WriteLine(RemoveList(itemNames, itemPrices, currCount));
    while (true)
    {
        string itemNoInp = Message("Enter the item number to remove or 0 to cancel: ");
        if (int.TryParse(itemNoInp, out int itemNo))
        {
            if (itemNo == 0)
            { Console.WriteLine("Reamoval cancled.\n"); break; }
            int index = itemNo - 1;
            if (TryRemoveItem(index, ref currCount, itemNames, itemPrices))
            { tipAmount = 0; Console.WriteLine("Item was successfully removed. Tips have been reset. \n"); break; }
        }

        Console.WriteLine($"Wrong input. Please enter a number between 1 and {currCount} (or 0).");
    }
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}


double AddTip(string menu, double subtotal)
{
    double tipChoice = Diap(menu, 1, 2);
    if (tipChoice == 0)
    {
        Console.WriteLine("Tip configuration canceled.\n");
        return 0;
    }
    else if (tipChoice == 1) return TipPercent(subtotal);
    else if (tipChoice == 2) return TipAmount();
    return 0;

}
double TipPercent(double subtotal)
{
    while (true)
    {
        string input = Message("Enter tip percentage (0-100%): ");
        if ((double.TryParse(input, out double percent)) && percent >= 0 && percent <= 100)
        {
            double totTip = subtotal * (percent / 100);
            return totTip;
        }
        Console.WriteLine("Wrong input. Percentage must be between 0 and 100.\n");
    }

}

double TipAmount()
{
    while (true)
    {
        string input = Message("Enter tip amount($): ");
        if ((double.TryParse(input, out double amount)) && amount > 0) return amount;
        Console.WriteLine("Wrong input. Amount must be more than 0.\n");
    }
}

string DisplayBill(string[] names, double[] prices, int count, double tipTot, double gst)
{
    if (count == 0) return "\n Bill is empty. No items to display.";

    int width = 38;
    string bill = $"\n{"Description",-22} {"Price",15}\n" +
                  new string('-', width) + "\n";

    double subtotal = 0;
    for (int i = 0; i < count; i++)
    {
        subtotal += prices[i];
        string priceFormatted = "$" + prices[i].ToString("F2");
        bill += $"{names[i],-22} {priceFormatted,15}\n";
    }
    bill += new string('-', width) + "\n";

    double gstCurr = Math.Round(subtotal * (gst / 100), 2);
    double total = CalculateTotal(subtotal, tipTot, gst);

    bill += $"{"Net Total",22} {"$" + subtotal.ToString("F2"),15}\n" +
            $"{"Tip Amount",22} {"$" + tipTot.ToString("F2"),15}\n" +
            $"{"GST Amount",22} {"$" + gstCurr.ToString("F2"),15}\n" +
            $"{"Total Amount",22} {"$" + total.ToString("F2"),15}\n\n"+
            new string('=', width) + "\n";
    return bill;
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