
Module Main
    Sub Main()
        '        'Dim Scan = New NetCoreMemoryScan
        '        'Dim Show = Scan.ScanAll("Tutorial-i386", "61 6E 64 6C 65 00 00 00 11 47 65 74")
        '        'Debug.WriteLine(Show.ToString)
        '        'Console.WriteLine(Show.ToString)

        '        'Dim MyAddress = 

        '        Dim Trm As New CheatEngine.Processor("terminal")
        '        'Dim Terminal0 = Trm.CEReadMemoryGetAddress("terminal.exe + 009ED164", "190 C 4 0 24 7C 67C 234")
        '        'Dim Terminal1 = Trm.CEReadMemoryGetAddress("terminal.exe + 009ED164", {&H190, &HC, &H4, &H0, &H24, &H7C, &H67C, &H234})
        '        'Dim Terminal2 = Trm.CEReadMemoryGetAddress(&H9ED164, {&H190, &HC, &H4, &H0, &H24, &H7C, &H67C, &H234})

        '        'Dim GetStringsTerminal As String = Trm.CEReadMemoryInt("terminal.exe + 009ED164", {&H190, &HC, &H4, &H0, &H24, &H7C, &H67C, &H234}, 100, 3)
        '        'Dim AddressAOB = Trm.AOBScanListStr("48 6F 6C 79 53 69 67 6E 61 6C 20 47 42 50 41 55 44 2C 4D 35 3A 20 41 6C 65 72 74 3A 20 32 30 32 34 2E 30 39 2E 30 36 20 32 30 3A 35 30 20 42 4F 5F 41 6C 65 72 74 20 44 6F 74 20 73 65 6C 6C 20 47 42 50 41 55 44 20 4D 35 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 42 00 65")

        '        Dim Replace = Trm.ReplaceBytesByte("48 6F 6C ?? ?? 69 67 6E 61 6C 20 47 42 50 41 55 44 2C 4D 35 3A 20 41 6C 65 72 74 3A 20 32 30 32 34 2E 30 39 2E 30 36 20 32 30 3A 35 30 20", "FF FF 6C 79 53 69 67 6E 61 6C 20 47 42 50 41 55 44 2C 4D 35 3A 20 41 6C 65 72 74 3A 20 32 30 32 34 2E 30 39 2E 30 36 20 32 30 3A 35 30 20")

        '        'Trm.

        '        End



        'Dim AOBs As New CheatEngine.Processor("Tutorial-i386")
        '' Dim stringo = AOBs.ReadMemoryInt("0019BB00", 4)



        'Dim result = AOBs.CEReadMemoryStr(&H242720, {"750", "C", "14", "14", "274", "34", "76C"}, 1)
        'Console.WriteLine(result)
        End





        '        Dim StringScan As String = "40 00 50 ?? 40 00 ?? CA 40 00 00"
        '        Dim StringScanResult As String = AOBs.AOBScanGetAddressStr(StringScan)

        '        Dim ScanResultIntPtr As IntPtr = AOBs.AOBScanGetAddressIntPtr(StringScan)

        '        Dim ListStringResult As List(Of String) = AOBs.AOBScanListStr(StringScan)

        '        'Dim ListBytesResult As List(Of IntPtr) = AOBs.AOBScanList(ByteScan)

        '        Dim h = HexStringToBytes("001EB632")

        '        Dim ReadMemoryString As String = AOBs.ReadMemoryStr("001EB632", 40)

        '        Dim ReadMemoryBytes As Byte = AOBs.ReadMemoryByte("001EB632", 3)

        '        Dim ReadMemoryintger As Integer = AOBs.ReadMemoryInt(&H19315C, 4)



        '        Dim WriteMemoryInteger As String = AOBs.WriteMemory("0019315C", 200)

        '        Dim Value As Byte() = BitConverter.GetBytes(400)
        '        Dim WriteMemoryByte As String = AOBs.WriteMemory("0019315C", Value)

        '        Dim Value1 As Byte() = BitConverter.GetBytes(5000)
        '        Dim WriteMemoryPtrByte As String = AOBs.WriteMemory(&H19315C, Value1)

        '        Dim WriteMemoryPtrString As String = AOBs.WriteMemory(&H19315C, "1000")

        '        Dim AddressBytes As Byte() = BitConverter.GetBytes(&H19315C)
        '        Dim WriteMemoryByteString As String = AOBs.WriteMemory(AddressBytes, "3000")

        '        Dim AddressBytes1 As Byte() = BitConverter.GetBytes(&H19315C)
        '        Dim WriteMemoryByteInteger As String = AOBs.WriteMemory(AddressBytes, 7000)


        '        Dim RString As String = AOBs.CEReadMemoryStr("Tutorial-i386.exe + 00242720", "750-C-14-14-274-34-76C", 100)
        '        Dim Rinteger As Integer = AOBs.CEReadMemoryInt("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
        '        Dim RDouble As Double = AOBs.CEReadMemoryDouble("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
        '        Dim RFloat As Single = AOBs.CEReadMemoryFloat("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)
        '        Dim Rbyte As Byte() = AOBs.CEReadMemoryBytes("Tutorial-i386.exe + 00242720", {&H750, &HC, &H14, &H14, &H274, &H34, &H76C}, 100)

        '        Dim MyAddress As Integer = &H2639DC
        '        Dim Load As String = AOBs.ReadMemoryStr(MyAddress, 50)
        '        Dim Loadint As Integer = AOBs.ReadMemoryInt(MyAddress, 50)
        '        Dim Loadfloat As Single = AOBs.ReadMemoryFloat(MyAddress, 50)
        '        Dim Loaddouble As Double = AOBs.ReadMemoryDouble(MyAddress, 50)
        '        Dim Loadbite As Byte() = AOBs.ReadMemoryByte(MyAddress, 50)
        '        Dim loadf As Single = AOBs.ReadMemoryFloat(&H386778, 20)
        '        Dim Loadd As Double = AOBs.ReadMemoryDouble(&H368DC4, 20)

        '        Dim GGRT = AOBs.ReplaceBytesStr("61 6E 64 6C 65 00 00 00 11 47 65 74", "60 6A 64 6C 65 00 00 00 11 47 65 74")

        '        Dim StringScanResultt As String = AOBs.AOBScanGetAddressStr("61 6E 64 6C 65 00 00 00 11 47 65 74")









        '        'Dim processoi As New Process

        '        'processoi = Process.GetProcessesByName("Tutorial-i386")(0)
        '        'Dim hProc = OpenProcess(&H242650, False, processoi.Id)
        '        'Dim kd = Process.GetProcessesByName("Tutorial-i386")(0).BasePriority
        '        'Dim dos = processoi.MainModule.BaseAddress
        '        'Dim grw3 = processoi.Modules
        '        'Dim modBase = GetModuleBaseAddress(processoi, "Tutorial-i386.exe")
        '        'Dim addr = FindDMAAddy(hProc, CType(modBase + &H10F4F4, IntPtr), New Integer() {&H374, &H14, 0})


        '        '  Dim LocalAddress As IntPtr = IntPtr.Add(processoi.MainModule.BaseAddress, &H242650)

        '        'Dim Vam As New VAMemory("Tutorial-i386")

        '        'Dim Address = Vam.ReadInt32(LocalAddress)

        '        ' Dim Helth = IntPtr.Add(Address, &H4AC)

        '        'Dim ReadInt = ReadInt32(LocalAddress)

        '        ' Dim Local As IntPtr = &H10332C

        '        ' Dim Read = AOBs.ReadMemory(Local, 100)
        '        ' Dim Helth = IntPtr.Add(Read, &H4AC)




        '        'Console.WriteLine("Bytes convertidos:")
        '        'For Each b As Byte In ByteScan
        '        '    Console.Write(b.ToString())
        '        'Next
        '        Console.WriteLine()



        '        'If address <> IntPtr.Zero Then
        '        '    Debug.WriteLine("Padrão encontrado no endereço: " & address.ToString("X"))
        '        'Else
        '        '    Debug.WriteLine("Padrão não encontrado.")
        '        'End If

        '        '  Debug.WriteLine(address.ToString)

    End Sub

    '    Public Function HexStringToBytes(ByVal hexString As String) As Byte()
    '        Dim hex = hexString.Replace(" ", "")
    '        Dim bytes = New Byte(hex.Length \ 2 - 1) {}
    '        For i As Integer = 0 To bytes.Length - 1
    '            bytes(i) = Convert.ToByte(hex.Substring(i * 2, 2), 16)
    '        Next
    '        Return bytes
    '    End Function
End Module
