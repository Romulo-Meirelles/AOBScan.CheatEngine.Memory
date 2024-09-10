# AOBScan.CheatEngine.Memory
created to explore items in memory and edit them using the Windows API. You can also scan AOB scan address list and replace them with other arrays. You can use it freely and spontaneously.

# ![Logo](https://raw.githubusercontent.com/Romulo-Meirelles/AOBScan.CheatEngine.Memory/main/Pictures/Memory.png) 
   AOBScan CheatEngine - (.NetFramework | .NetStandard | .NetCore)

[![NuGet version (AOBScan)](https://img.shields.io/nuget/v/AOBScan.CheatEngine.Memory.svg?style=flat-square)](https://www.nuget.org/packages/AOBScan.CheatEngine.Memory/)

AOBSCAN FOR CHEATS AND MEMORY SCANNER READ AND WRITER.

So you can scan the memory and find what you are looking for, values, signals, address, pointer, etc...

- Features
- AOB
- Replacer Arrays
- Writter
- Read.
- Int.
- Float.
- Double.
- Bytes.
- Address.
- Pointer.

#HAVE FUN!

##  SOME EXAMPLES OF HOW TO USE IT.

## Visual Basic

```vb
Using Client = BitnuvemClient.Create(New ApiConfig With {.ApiKey = "25g7i3e6d17835bdf136d8ab2efv94er", .SecretKey = "8ur15dXbawTu9nUzyYpfFBaXiq3iR1ff"})
Try
 'PEGA O SEU BALANÇO.
 Dim Balance = Await Client.Balance
 Console.WriteLine(Balance.ToString)
 Console.WriteLine()
 
 'CRIA UMA NOVA ORDEM DE VENDA OU COMPRA LIMITE.
 Dim VendaLimite = Await Client.Order_New_Limite(BitnuvemClient.Order.Venda, "0.00410880", "95494.72")
 Console.WriteLine(VendaLimite.ToString)
 Console.WriteLine()
 
 'LISTA TODAS AS ORDEM (ATIVAS, CANCELADAS OU PENDENTES).
 Dim GetOrderList = Await Client.Order_List(BitnuvemClient.OrderList.Ativas)
 Console.WriteLine(GetOrderList.ToString)
 Console.WriteLine()
             
 'CANCELA UMA ORDEM DE COMPRA OU VENDA COM IDENTIFICAÇÃO           
 Dim CancelOrder = Await Client.Order_Cancel("48783624")
 Console.WriteLine(CancelOrder.ToString)
 Console.WriteLine()
 
 'CANCELA TODAS AS ORDENS QUE ESTIVEREM ABERTAS.
 Dim CancelAllOrder = Await Client.Order_Cancel_All(BitnuvemClient.OrderType.Venda)
 Console.WriteLine(CancelAllOrder.ToString)
 Console.WriteLine()
 
Catch ex As Exception
Console.WriteLine(ex.Message)
End Try
```


## Links

- [Homepage](https://github.com/Romulo-Meirelles)
- [NuGet Package](https://www.nuget.org/packages/AOBScan.CheatEngine.Memory)
- [Github Project](https://github.com/Romulo-Meirelles)
- [License](https://github.com/Romulo-Meirelles/AOBScan.CheatEngine.Memory/blob/main/LICENSE)
- [Telegram](https://t.me/TMS_40)
- [WhatsApp](https://wa.me/message/KWIS3BYO6K24N1)
- [UpWork](https://www.upwork.com/freelancers/~01fcbc5039ac5766b4)
