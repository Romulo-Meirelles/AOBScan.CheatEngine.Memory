# AOBScan.CheatEngine.Memory
created to explore items in memory and edit them using the Windows API. You can also scan AOB scan address list and replace them with other arrays. You can use it freely and spontaneously.

# ![Logo](https://raw.githubusercontent.com/Romulo-Meirelles/AOBScan.CheatEngine.Memory/main/Pictures/Memory.png) 
   AOBScan CheatEngine - (.NetFramework | .NetStandard | .NetCore)

[![NuGet version (AOBScan)](https://img.shields.io/nuget/v/AOBScan.CheatEngine.Memory.svg?style=flat-square)](https://www.nuget.org/packages/AOBScan.CheatEngine.Memory/)

AOBSCAN FOR CHEATS AND MEMORY SCANNER READ AND WRITER.

So you can scan the memory and find what you are looking for, values, signals, address, pointer, etc...

- Features.
- AOB Scan.
- Replacer Arrays.
- Writter.
- Read.
- String.
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
IMPORTS AOBScan.CheatEngine

Dim Game As New CheatEngine.Processor("Tutorial-i386")

Try

'GET THE REQUESTED ADDRESS.
 Dim Address0 = Game.CEReadMemoryGetAddress("Tutorial-i386 + 009ED164", "190 C 4 0 24 7C 67C 234")
 Dim Address1 = Game.CEReadMemoryGetAddress(&H009ED164, "190 C 4 0 24 7C 67C 234")
 Dim Address2 = Game.CEReadMemoryGetAddress(&H009ED164, {"190", "C", "4", "0", "24", "7C", "67C", "234"})
 Dim Address3 = Game.CEReadMemoryGetAddress(&H009ED164, {&H190, &HC, &H4, &H0, &H24, &H7C, &H67C, &H234})

'LIST ALL ADDRESSES FOUND BY THE RESQUESTED BYTES.
Dim ListInt As List(Of String) = Game.AOBScanListStr("48 6F 6C 79 53 69 67 6E 61 6C 20 47")

'REPLACES AN ARRAY OF BYTES WITH ANOTHER ARRAY OF BYTES.
Dim Replace As IntPtr = Game.ReplaceBytesByte("48 6F 6C ?? ?? 69 67 6E 61 6C", "FF FF 6C 79 53 69 67 6E 61 6C")

READ A SPECIFIC ITEM IN MEMORY USING POINTER AND OFFSETS.
Dim ReadStr As String = Game.CEReadMemoryStr("Tutorial-i386.exe + 00242720", "750-C-14-14-274-34-76C", 100)
Dim ReadInt As Integer = Game.CEReadMemoryInt("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
Dim ReadDouble As Double = Game.CEReadMemoryDouble("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
Dim ReadFloat As Single = Game.CEReadMemoryFloat(&H00242720, {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
Dim ReadBytes As Byte() = Game.CEReadMemoryBytes(&H00242720, {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)

'NORMAL RADDING WITH ADDRESS PROVIDED RETURN IN STRING, INTEGER, FLOAT, DOUBLE AND BYTE.
Dim MyAddress As Integer = &H2639DC
Dim LoadStr As String = Game.ReadMemoryStr(MyAddress, 50)
Dim Loadint As Integer = Game.ReadMemoryInt(MyAddress, 50)
Dim LoadFloat As Single = Game.ReadMemoryFloat(MyAddress, 50)
Dim LoadDouble As Double = Game.ReadMemoryDouble(MyAddress, 50)
Dim LoadByte As Byte() = Game.ReadMemoryByte(MyAddress, 50)

Dim LoadFloat As Single = Game.ReadMemoryFloat(&H2639DC, 50)
Dim LoadDouble As Double = Game.ReadMemoryDouble("002639DC", 50)
Dim LoadByte As Byte() = Game.ReadMemoryByte("002639DC", 50)

'WRITTEN INTO MEMORY WITH A SIMPLE AMD MODIFIED ADDRESS. DESIRED RETURN.
Dim WriteMemoryInt As Integer = Game.WriteMemory("0019315C", 300)
Dim WriteMemoryByte As Byte = Game.WriteMemory("0019315C", BitConverter.GetBytes(400))
Dim WriteMemoryPtr As IntPtr = Game.WriteMemory(&H19315C, BitConverter.GetBytes(500))
Dim WriteMemoryStr As String = Game.WriteMemory(&H19315C, "1000")
Dim WriteMemoryFloat As Single = Game.WriteMemory(BitConverter.GetBytes(&H19315C), BitConverter.GetBytes("Life"))
Dim WriteMemoryStr As Double = Game.WriteMemory(&H19315C, "2000")

Catch ex As Exception
Console.WriteLine(ex.Message)
End Try
```

## C# CSharp

```csharp
using AOBScan.CheatEngine;
using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        CheatEngine.Processor Game = new CheatEngine.Processor("Tutorial-i386");

        try
        {
            // GET THE REQUESTED ADDRESS.
            IntPtr Address0 = Game.CEReadMemoryGetAddress("Tutorial-i386 + 009ED164", "190 C 4 0 24 7C 67C 234");
            IntPtr Address1 = Game.CEReadMemoryGetAddress((IntPtr)0x009ED164, "190 C 4 0 24 7C 67C 234");
            IntPtr Address2 = Game.CEReadMemoryGetAddress((IntPtr)0x009ED164, new string[] { "190", "C", "4", "0", "24", "7C", "67C", "234" });
            IntPtr Address3 = Game.CEReadMemoryGetAddress((IntPtr)0x009ED164, new int[] { 0x190, 0xC, 0x4, 0x0, 0x24, 0x7C, 0x67C, 0x234 });

            // LIST ALL ADDRESSES FOUND BY THE REQUESTED BYTES.
            List<string> ListInt = Game.AOBScanListStr("48 6F 6C 79 53 69 67 6E 61 6C 20 47");

            // REPLACES AN ARRAY OF BYTES WITH ANOTHER ARRAY OF BYTES.
            IntPtr Replace = Game.ReplaceBytesByte("48 6F 6C ?? ?? 69 67 6E 61 6C", "FF FF 6C 79 53 69 67 6E 61 6C");

            // READ A SPECIFIC ITEM IN MEMORY USING POINTER AND OFFSETS.
            string ReadStr = AOBs.CEReadMemoryStr("Tutorial-i386.exe + 00242720", "750-C-14-14-274-34-76C", 100);
            int ReadInt = AOBs.CEReadMemoryInt("Tutorial-i386.exe + 00242720", new int[] { 0x750, 0xC, 0x14, 0x14, 0x274, 0x34, 0x76C }, 100);
            double ReadDouble = AOBs.CEReadMemoryDouble("Tutorial-i386.exe + 00242720", new int[] { 0x750, 0xC, 0x14, 0x14, 0x274, 0x34, 0x76C }, 100);
            float ReadFloat = AOBs.CEReadMemoryFloat((IntPtr)0x00242720, new int[] { 0x750, 0xC, 0x14, 0x14, 0x274, 0x34, 0x76C }, 100);
            byte[] ReadBytes = AOBs.CEReadMemoryBytes((IntPtr)0x00242720, new int[] { 0x750, 0xC, 0x14, 0x14, 0x274, 0x34, 0x76C }, 100);

            // NORMAL READING WITH ADDRESS PROVIDED RETURN IN STRING, INTEGER, FLOAT, DOUBLE AND BYTE.
            int MyAddress = 0x2639DC;
            string LoadStr = AOBs.ReadMemoryStr(MyAddress, 50);
            int LoadInt = AOBs.ReadMemoryInt(MyAddress, 50);
            float LoadFloat = AOBs.ReadMemoryFloat(MyAddress, 50);
            double LoadDouble = AOBs.ReadMemoryDouble(MyAddress, 50);
            byte[] LoadByte = AOBs.ReadMemoryByte(MyAddress, 50);

            LoadFloat = AOBs.ReadMemoryFloat(0x2639DC, 50);
            LoadDouble = AOBs.ReadMemoryDouble("002639DC", 50);
            LoadByte = AOBs.ReadMemoryByte("002639DC", 50);

            // WRITTEN INTO MEMORY WITH A SIMPLE AND MODIFIED ADDRESS. DESIRED RETURN.
            int WriteMemoryInt = Game.WriteMemory("0019315C", 300);
            byte[] WriteMemoryByte = Game.WriteMemory("0019315C", BitConverter.GetBytes(400));
            IntPtr WriteMemoryPtr = Game.WriteMemory((IntPtr)0x19315C, BitConverter.GetBytes(500));
            string WriteMemoryStr = Game.WriteMemory((IntPtr)0x19315C, "1000");
            float WriteMemoryFloat = Game.WriteMemory(BitConverter.GetBytes((int)0x19315C), BitConverter.GetBytes(1000f));
            double WriteMemoryDouble = Game.WriteMemory((IntPtr)0x19315C, "2000");   
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

```


## Links

- [Homepage](https://github.com/Romulo-Meirelles)
- [NuGet Package](https://www.nuget.org/packages/AOBScan.CheatEngine.Memory)
- [Github Project](https://github.com/Romulo-Meirelles/AOBScan.CheatEngine.Memory)
- [License](https://github.com/Romulo-Meirelles/AOBScan.CheatEngine.Memory/blob/main/LICENSE)
- [Telegram](https://t.me/RMS_40)
- [WhatsApp](https://wa.me/message/KWIS3BYO6K24N1)
- [UpWork](https://www.upwork.com/freelancers/~01fcbc5039ac5766b4)
