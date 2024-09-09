Imports System.Runtime.InteropServices
Imports AOBScan.EnablePrivileges
Imports AOBScan.Memory

Namespace DMA

    ''' <summary>
    ''' *** MADE FOR GITHUB ***
    ''' WRITTEN BY ROMULO MEIRELLES.
    ''' UPWORK: https://www.upwork.com/freelancers/~01fcbc5039ac5766b4
    ''' CONTACT WHATSAPP: https://wa.me/message/KWIS3BYO6K24N1
    ''' CONTACT TELEGRAM: https://t.me/RMS_40
    ''' GITHUB: https://github.com/Romulo-Meirelles
    ''' DISCORD: đąяķňέs§¢øďε#2786
    ''' E-Mail: fawkeman@protonmail.com
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Public Class ReadWritingMemory

        Const PROCESS_ALL_ACCESS = &H1F0FF
        Private Property Proc As Process
        Sub New(Process As Process)
            GoDebugPriv()
            Proc = Process
        End Sub
        Sub New(ProcessName As String)
            GoDebugPriv()
            ProcessName = ProcessName.Replace(".exe", "").Replace(".dll", "").Replace(".com", "")
            Proc = Process.GetProcessesByName(ProcessName)(0)
        End Sub
        Sub New(ProcessPID As Integer)
            GoDebugPriv()
            Proc = Process.GetProcessById(ProcessPID)
        End Sub


#Region "READER"
        Public Function ReadDMAInteger(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Integer
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadInteger(lvl, nsize) + Offsets(i - 1)
                Next
                Dim vBuffer As Integer
                vBuffer = ReadInteger(lvl, nsize)
                Return vBuffer
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadDMAFloat(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Single
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadFloat(lvl, nsize) + Offsets(i - 1)
                Next
                Dim vBuffer As Single
                vBuffer = ReadFloat(lvl, nsize)
                Return vBuffer
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadDMALong(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Long
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadLong(lvl, nsize) + Offsets(i - 1)
                Next
                Dim vBuffer As Long
                vBuffer = ReadLong(lvl, nsize)
                Return vBuffer
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadInteger(ByVal Address As Integer, Optional ByVal nsize As Integer = 4) As Integer

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Return 0
                Exit Function
            End If

            Dim hAddress, vBuffer As Integer
            hAddress = Address
            ReadProcessMemory1(hProcess, hAddress, vBuffer, nsize, 0)
            Return vBuffer
        End Function
        Public Function ReadFloat(ByVal Address As Integer, Optional ByVal nsize As Integer = 4) As Single

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Return 0
                Exit Function
            End If

            Dim hAddress As Integer
            Dim vBuffer As Single

            hAddress = Address
            ReadProcessMemory2(hProcess, hAddress, vBuffer, nsize, 0)
            Return vBuffer
        End Function
        Public Function ReadLong(ByVal Address As Integer, Optional ByVal nsize As Integer = 4) As Long

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Return 0
                Exit Function
            End If

            Dim hAddress As Integer
            Dim vBuffer As Long

            hAddress = Address
            ReadProcessMemory3(hProcess, hAddress, vBuffer, nsize, 0)
            Return vBuffer
        End Function

#End Region

#Region "WRITER"
        Public Function WriteDMAInteger(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Value As Integer, ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Boolean
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadInteger(lvl, nsize) + Offsets(i - 1)
                Next
                WriteInteger(lvl, Value, nsize)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Function WriteDMAFloat(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Value As Single, ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Boolean
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadFloat(lvl, nsize) + Offsets(i - 1)
                Next
                WriteFloat(lvl, Value, nsize)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Function WriteDMALong(ByVal Address As Integer, ByVal Offsets As Integer(), ByVal Value As Long, ByVal Level As Integer, Optional ByVal nsize As Integer = 4) As Boolean
            Try
                Dim lvl As Integer = Address
                For i As Integer = 1 To Level
                    lvl = ReadLong(lvl, nsize) + Offsets(i - 1)
                Next
                WriteLong(lvl, Value, nsize)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Sub WriteNOPs(ByVal Address As Long, ByVal NOPNum As Integer)
            Dim C As Integer
            Dim B As Integer
            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Exit Sub
            End If

            B = 0
            For C = 1 To NOPNum
                Call WriteProcessMemory1(hProcess, Address + B, &H90, 1, 0&)
                B = B + 1
            Next C
        End Sub
        Public Sub WriteXBytes(ByVal Address As Long, ByVal Value As String)
            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Exit Sub
            End If

            Dim C As Integer
            Dim B As Integer
            Dim D As Integer
            Dim V As Byte

            B = 0
            D = 1
            For C = 1 To Math.Round((Len(Value) / 2))
                V = Val("&H" & Mid$(Value, D, 2))
                Call WriteProcessMemory1(hProcess, Address + B, V, 1, 0&)
                B = B + 1
                D = D + 2
            Next C

        End Sub
        Public Sub WriteInteger(ByVal Address As Integer, ByVal Value As Integer, Optional ByVal nsize As Integer = 4)

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Exit Sub
            End If

            Dim hAddress, vBuffer As Integer
            hAddress = Address
            vBuffer = Value
            WriteProcessMemory1(hProcess, hAddress, CInt(vBuffer), nsize, 0)
        End Sub
        Public Sub WriteFloat(ByVal Address As Integer, ByVal Value As Single, Optional ByVal nsize As Integer = 4)

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Exit Sub
            End If

            Dim hAddress As Integer
            Dim vBuffer As Single

            hAddress = Address
            vBuffer = Value
            WriteProcessMemory2(hProcess, hAddress, vBuffer, nsize, 0)
        End Sub
        Public Sub WriteLong(ByVal Address As Integer, ByVal Value As Long, Optional ByVal nsize As Integer = 4)

            Dim hProcess As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, 0, Proc.Id)
            If hProcess = IntPtr.Zero Then
                Exit Sub
            End If

            Dim hAddress As Integer
            Dim vBuffer As Long

            hAddress = Address
            vBuffer = Value
            WriteProcessMemory3(hProcess, hAddress, vBuffer, nsize, 0)
        End Sub

#End Region
        Public Function FindDMAAddy(hProc As IntPtr, ptr As IntPtr, offsets As Integer()) As IntPtr
            Dim buffer As Byte() = New Byte(IntPtr.Size - 1) {}

            For Each i As Integer In offsets
                ReadProcessMemory0(hProc, ptr, buffer, buffer.Length, Nothing)

                ptr = If(IntPtr.Size = 4,
                 IntPtr.Add(New IntPtr(BitConverter.ToInt32(buffer, 0)), i),
                 IntPtr.Add(New IntPtr(BitConverter.ToInt64(buffer, 0)), i))
            Next

            Return ptr
        End Function
        Public Function GetModuleBaseAddress(modName As String) As IntPtr
            Dim addr As IntPtr = IntPtr.Zero

            For Each m As ProcessModule In Proc.Modules
                If m.ModuleName = modName Then
                    addr = m.BaseAddress
                    Exit For
                End If
            Next

            Return addr
        End Function
        Function ReturnClient(ByVal Processor As String) As System.IntPtr
            Try
                Dim pc As Process = Process.GetProcessesByName(Processor)(0)
                For Each pm As ProcessModule In pc.Modules
                    If pm.ModuleName = Processor & ".exe" Then Return pm.BaseAddress
                Next
            Catch ex As Exception
                Return IntPtr.Zero
            End Try
        End Function

    End Class
End Namespace