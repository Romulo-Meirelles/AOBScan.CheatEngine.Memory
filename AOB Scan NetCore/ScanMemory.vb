'Imports System
'Imports System.Collections.Generic
'Imports System.Linq
'Imports System.Text
'Imports System.Threading.Tasks
'Imports System.Runtime.InteropServices
'Imports System.Diagnostics
'Imports System.IO
'Imports Microsoft.Win32.SafeHandles
'Imports System.Text.RegularExpressions
'Imports System.Runtime.ConstrainedExecution
'Imports System.Security
'Imports System.Security.Principal


'Public Class NetCoreMemoryScan : Inherits EnablePrivileges

'#Region "System Dlls"

'#End Region

'    Private current_aob As Byte() = Nothing
'    Private mask As String = ""
'    Private handle As IntPtr = IntPtr.Zero
'    Private pid As Integer = 0

'    Public Sub New()
'        Call GoDebugPriv()
'    End Sub
'    Public Shared Function GetSystemMessage(errorCode As UInteger) As String
'        Dim exception = New System.ComponentModel.Win32Exception(CInt(errorCode))
'        Return exception.Message
'    End Function
'    Private Function is_valid_hex_array(text As String) As Boolean
'        Dim regex = New Regex("^([a-fA-F0-9]{2}?(.*\?)?\s?)+$")
'        Dim match = regex.Match(text)
'        Return match.Success
'    End Function

'    Private Function is_valid_pattern_mask(text As String) As Boolean
'        Dim regex = New Regex("^([\\*][x][a-fA-F0-9]{2})+$")
'        Dim match = regex.Match(text)
'        Return match.Success
'    End Function

'    Private Function is_valid_mask(text As String) As Boolean
'        Dim regex = New Regex("^([xX]?(.*\?)?)+$")
'        Dim match = regex.Match(text)
'        Return match.Success
'    End Function

'    Private Function str_array_to_aob(inputed_str As String) As Integer
'        Dim trated_str = inputed_str.Replace("  ", "")

'        trated_str = If(trated_str(0) = " "c, trated_str.Substring(1, trated_str.Length - 1), trated_str)

'        trated_str = If(trated_str.Substring(trated_str.Length - 1, 1) = " "c, trated_str.Substring(0, trated_str.Length - 1), trated_str)

'        If Not is_valid_hex_array(trated_str) Then
'            Debug.Write("not valid hex array {x1F0}", "by NetCoreMemoryScan")
'            Return 0
'        End If

'        mask = ""
'        Dim part_hex = inputed_str.Split(" "c)
'        current_aob = New Byte(part_hex.Count() - 1) {}

'        For i = 0 To part_hex.Count() - 1
'            If part_hex(i).Contains("?") Then
'                current_aob(i) = &HCC
'                mask += "?"
'            Else
'                current_aob(i) = Convert.ToByte(part_hex(i), 16)
'                mask += "x"
'            End If
'        Next
'        Return part_hex.Count()
'    End Function

'    Private Function pattern_to_aob(inputed_str As String, i_mask As String) As Integer
'        If Not is_valid_mask(i_mask) Then Return 0

'        Dim trated_str = inputed_str.Replace(" ", "")
'        If Not is_valid_pattern_mask(trated_str) Then
'            Debug.Write("not valid pattern {x1F0}", "by NetCoreMemoryScan")
'            Return 0
'        End If

'        Dim part_hex = inputed_str.Split(New String() {"\x"}, StringSplitOptions.None)
'        If (part_hex.Count() - 1) <> i_mask.Length Then Return 0

'        mask = i_mask
'        current_aob = New Byte(part_hex.Count() - 2) {}
'        For i = 1 To part_hex.Count() - 1
'            Dim l = i - 1
'            If i_mask(l) = "?"c Then
'                current_aob(l) = &HCC
'            Else
'                current_aob(l) = Convert.ToByte(part_hex(i), 16)
'            End If
'        Next
'        Return part_hex.Count()
'    End Function

'    Private Function pattern_to_aob(inputed_str As String) As Integer
'        Dim trated_str = inputed_str.Replace(" ", "")
'        If Not is_valid_pattern_mask(trated_str) Then
'            Debug.Write("not valid pattern {x1F1}", "by NetCoreMemoryScan")
'            Return 0
'        End If

'        Dim part_hex = inputed_str.Split(New String() {"\x"}, StringSplitOptions.None)
'        current_aob = New Byte(part_hex.Count() - 2) {}
'        For i = 1 To part_hex.Count() - 1
'            current_aob(i - 1) = Convert.ToByte(part_hex(i), 16)
'        Next
'        Return part_hex.Count()
'    End Function

'    Public Shared Function IsAdministrator() As Boolean
'        If Not RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
'            Throw New PlatformNotSupportedException("Esta função só é suportada no Windows.")
'        End If
'        Dim identity = System.Security.Principal.WindowsIdentity.GetCurrent()
'        Dim principal = New WindowsPrincipal(identity)
'        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
'    End Function

'    Private Function get_handle(p As Process) As IntPtr
'        If p Is Nothing Then Return IntPtr.Zero

'        Try
'            Return p.Handle
'        Catch ex As Exception
'            If Not IsAdministrator() Then
'                Debug.Write("Run the program as an administrator.", "by NetCoreMemoryScan")
'            Else
'                Debug.Write("error: " & ex.Message)
'            End If
'        End Try

'        Return IntPtr.Zero
'    End Function

'    Function ScanAllRegions(ByVal pattern As String, ByVal mask As String) As IntPtr
'        If PatternToAOB(pattern, mask) = 0 Then
'            Return IntPtr.Zero
'        End If
'        Me.handle = handle
'        Return ScanAllRegions()
'    End Function

'    Function ScanAllRegions() As IntPtr
'        If Is_X64_Process(Process.GetCurrentProcess().Handle) <> Is_X64_Process(Me.handle) Then
'            Debug.WriteLine("Problemas com a retenção de informações ou incompatibilidade arquitetônica com o processo alvo.", "por NetCoreMemoryScan")
'            Return IntPtr.Zero
'        End If

'        Dim mappedMemory As New List(Of MEMORY_BASIC_INFORMATION)()
'        If Not Map_Process_Memory(handle, mappedMemory) Then
'            Return IntPtr.Zero
'        End If

'        For i As Integer = 0 To mappedMemory.Count() - 1
'            Dim buffer As Byte() = New Byte(CUInt(mappedMemory(i).RegionSize) - 1) {}
'            Dim numberOfBytesRead As UInteger

'            If ReadProcessMemory(handle, mappedMemory(i).BaseAddress, buffer, CUInt(mappedMemory(i).RegionSize), numberOfBytesRead) AndAlso numberOfBytesRead > 0 Then
'                Dim ret = Search_Pattern(buffer, 0)
'                If ret <> 0 Then
'                    Return CType(mappedMemory(i).BaseAddress.ToInt64() + ret, IntPtr)
'                End If
'            End If

'            Dim errorCode = GetLastError()
'            If errorCode = 6 Then ' Às vezes, o .NET fecha o handle.
'                Dim p = Process.GetProcessById(pid)
'                If p IsNot Nothing Then
'                    Me.handle = p.Handle
'                End If
'            End If
'        Next

'        Return IntPtr.Zero
'    End Function
'    Private Function PatternToAob(inputedStr As String, iMask As String) As Integer
'        If Not is_valid_mask(iMask) Then
'            Return 0
'        End If

'        Dim tratedStr As String = inputedStr.Replace(" ", "")
'        If Not is_valid_pattern_mask(tratedStr) Then
'            Debug.WriteLine("not valid pattern {x1F0}", "by NetCoreMemoryScan")
'            Return 0
'        End If

'        Dim partHex() As String = inputedStr.Split(New String() {"\x"}, StringSplitOptions.None)
'        If (partHex.Length - 1) <> iMask.Length Then
'            Return 0
'        End If

'        Me.mask = iMask
'        Me.current_aob = New Byte(partHex.Length - 2) {}

'        For i As Integer = 1 To partHex.Length - 1
'            Dim l As Integer = i - 1
'            If iMask(l) = "?"c Then
'                Me.current_aob(l) = &HCC
'            Else
'                Me.current_aob(l) = Convert.ToByte(partHex(i), 16)
'            End If
'        Next

'        Return partHex.Length
'    End Function

'    Private Function PatternToAob(inputedStr As String) As Integer
'        Dim tratedStr As String = inputedStr.Replace(" ", "")
'        If Not is_valid_pattern_mask(tratedStr) Then
'            Debug.WriteLine("not valid pattern {x1F1}", "by dotNetMemoryScan")
'            Return 0
'        End If

'        Dim partHex() As String = inputedStr.Split(New String() {"\x"}, StringSplitOptions.None)
'        Me.current_aob = New Byte(partHex.Length - 2) {}

'        For i As Integer = 1 To partHex.Length - 1
'            Me.current_aob(i - 1) = Convert.ToByte(partHex(i), 16)
'        Next

'        Return partHex.Length
'    End Function


'#Region "With Mask"
'    Public Function ScanAll(handle As IntPtr, pattern As String, mask As String) As IntPtr
'        If PatternToAob(pattern, mask) = 0 Then
'            Return IntPtr.Zero
'        End If
'        Me.handle = handle
'        Return ScanAllRegions()
'    End Function

'    Public Function ScanAll(p As Process, pattern As String, mask As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(p)
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern, mask)
'        End If
'        Return IntPtr.Zero
'    End Function

'    Public Function ScanAll(pName As String, pattern As String, mask As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(GetPID(pName.Replace(".exe", "")))
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern, mask)
'        End If
'        Return IntPtr.Zero
'    End Function

'    Public Function ScanAll(pid As Integer, pattern As String, mask As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(Process.GetProcessById(pid))
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern, mask)
'        End If
'        Return IntPtr.Zero
'    End Function
'#End Region


'#Region "Without Mask"
'    Public Function ScanAll(handle As IntPtr, pattern As String) As IntPtr
'        If str_array_to_aob(pattern) = 0 Then
'            Return IntPtr.Zero
'        End If
'        Me.handle = handle
'        Me.pid = GetProcessId(Me.handle)
'        Return ScanAllRegions()
'    End Function

'    Public Function ScanAll(p As Process, pattern As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(p)
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern)
'        End If
'        Return IntPtr.Zero
'    End Function

'    Public Function ScanAll(pName As String, pattern As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(GetPID(pName.Replace(".exe", "")))
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern)
'        End If
'        Return IntPtr.Zero
'    End Function

'    Public Function ScanAll(pid As Integer, pattern As String) As IntPtr
'        Dim byHandle As IntPtr = get_handle(Process.GetProcessById(pid))
'        If byHandle <> IntPtr.Zero Then
'            Return ScanAll(byHandle, pattern)
'        End If
'        Return IntPtr.Zero
'    End Function

'#End Region

'#Region "Get PIDs"
'    Private Function GetPID(pid As Integer) As Process
'        Try
'            Return Process.GetProcessById(pid)
'        Catch ex As Exception
'            Debug.Write("process not found.", "by NetCoreMemoryScan")
'            Return Nothing
'        End Try
'    End Function

'    Private Function GetPID(name As String) As Process
'        If name = "" Then
'            Debug.Write("name not found.", "by NetCoreMemoryScan")
'            Return Nothing
'        End If

'        Try
'            Dim p = Process.GetProcessesByName(name)
'            Return p(0)
'        Catch ex As Exception
'            Debug.Write("process not found.", "by NetCoreMemoryScan")
'            Return Nothing
'        End Try
'    End Function

'    Private Function GetPID(ByRef processes() As Process, name As String) As Process
'        Try
'            For i = 0 To processes.Length - 1
'                If processes(i).ProcessName = name Then Return processes(i)
'            Next
'        Catch ex As Exception
'            Debug.Write("process not found.", "by NetCoreMemoryScan")
'        End Try

'        Return Nothing
'    End Function

'#End Region


'    Private Function Is_X64_Process(byHandle As IntPtr) As Integer
'        Dim is64 As Boolean = False
'        If Not IsWow64Process(byHandle, is64) Then
'            Return -1
'        End If
'        Return Convert.ToInt32(Not is64)
'    End Function

'    Protected Function Map_Process_Memory(pHandle As IntPtr, ByRef mappedMemory As List(Of MEMORY_BASIC_INFORMATION)) As Boolean
'        Dim address As IntPtr = IntPtr.Zero
'        Dim MBI As New MEMORY_BASIC_INFORMATION()

'        Dim found As UInteger = VirtualQueryEx(pHandle, address, MBI, CUInt(Marshal.SizeOf(MBI)))
'        While found <> 0
'            If (MBI.State And CUInt(StateEnum.MEM_COMMIT)) <> 0 AndAlso (MBI.Protect And CUInt(AllocationProtectEnum.PAGE_GUARD)) <> CUInt(AllocationProtectEnum.PAGE_GUARD) Then
'                mappedMemory.Add(MBI)
'            End If
'            address = New IntPtr(MBI.BaseAddress.ToInt64() + CUInt(MBI.RegionSize))
'            found = VirtualQueryEx(pHandle, address, MBI, CUInt(Marshal.SizeOf(MBI)))
'        End While

'        Return mappedMemory.Count > 0
'    End Function

'    Private Function Search_Pattern(buffer As Byte(), initIndex As Integer) As Integer
'        For i As Integer = initIndex To buffer.Length - 1
'            For x As Integer = 0 To current_aob.Length - 1
'                If current_aob(x) <> buffer(i + x) AndAlso mask(x) <> "?"c Then
'                    Exit For
'                End If
'            Next
'            Return i
'            Exit For
'        Next
'        Return 0
'    End Function

'    Private Function find_aob_pattern(buffer() As Byte) As Integer
'        Dim patternLength = current_aob.Length
'        For i = 0 To buffer.Length - patternLength
'            Dim match = True
'            For j = 0 To patternLength - 1
'                If mask(j) = "x"c AndAlso buffer(i + j) <> current_aob(j) Then
'                    match = False
'                    Exit For
'                End If
'            Next
'            If match Then Return i
'        Next
'        Return -1
'    End Function


'End Class
