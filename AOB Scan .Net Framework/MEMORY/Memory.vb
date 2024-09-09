Imports System.Runtime.InteropServices
Friend Class Memory

    '<DllImport("advapi32.dll", SetLastError:=True)>
    Protected Friend Declare Function OpenProcessToken Lib "advapi32" Alias "OpenProcessToken" (ByVal ProcessHandle As IntPtr, ByVal DesiredAccess As UInt32, ByRef TokenHandle As IntPtr) As Boolean
    'End Function

    '<DllImport("kernel32.dll", SetLastError:=True)>
    Protected Friend Declare Function GetCurrentProcess Lib "kernel32" Alias "GetCurrentProcess" () As IntPtr
    ' End Function

    '<DllImport("kernel32.dll")>
    Protected Friend Declare Function GetLastError Lib "kernel32" Alias "GetLastError" () As UInteger
    'End Function

    '<DllImport("kernel32.dll", SetLastError:=True)>
    Protected Friend Declare Function SetLastError Lib "kernel32" Alias "SetLastError" (dwErrorCode As UInteger)
    'End Sub

    '<DllImport("kernel32.dll")>
    Protected Friend Declare Function OpenProcess Lib "kernel32" Alias "OpenProcess" (dwDesiredAccess As UInteger, bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
    'End Function

    '<DllImport("kernel32.dll")>
    Protected Friend Declare Function ReadProcessMemory Lib "kernel32" Alias "ReadProcessMemory" (hProcess As IntPtr, lpBaseAddress As IntPtr, buffer As Byte(), size As UInteger, ByRef lpNumberOfBytesRead As UInteger) As Boolean
    'End Function

    ' <DllImport("kernel32.dll")>
    Protected Friend Declare Function WriteProcessMemory Lib "kernel32" Alias "WriteProcessMemory" (hProcess As IntPtr, lpBaseAddress As IntPtr, buffer As Byte(), size As UInteger, ByRef lpNumberOfBytesWritten As UInteger) As Boolean
    'End Function
    '<DllImport("kernel32.dll", SetLastError:=True)>
    Protected Friend Declare Function VirtualQueryEx Lib "kernel32" Alias "VirtualQueryEx" (hProcess As IntPtr, lpAddress As IntPtr, ByRef lpBuffer As MEMORY_BASIC_INFORMATION, dwLength As UInteger) As Integer
    'End Function

    '<DllImport("kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)>
    Protected Friend Declare Function IsWow64Process Lib "kernel32" Alias "IsWow64Process" (ByVal processHandle As IntPtr, <MarshalAs(UnmanagedType.Bool)> ByRef wow64Process As Boolean) As Boolean
    ' End Function

    ' <DllImport("kernel32.dll", EntryPoint:="GetProcessId", CharSet:=CharSet.Auto)>
    Protected Friend Declare Function GetProcessId Lib "kernel32" Alias "GetProcessId" (handle As IntPtr) As Integer
    ' End Function

    '<DllImport("kernel32.dll")>
    Protected Friend Declare Function VirtualProtectEx Lib "kernel32" Alias "VirtualProtectEx" (hProcess As IntPtr, lpAddress As IntPtr, dwSize As UIntPtr, flNewProtect As UInteger, ByRef lpflOldProtect As UInteger) As Boolean
    ' End Function

    ' <DllImport("kernel32.dll", SetLastError:=True)>
    Protected Friend Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hHandle As IntPtr) As Boolean
    ' End Function


    Protected Friend Declare Function WriteProcessMemory1 Lib "kernel32" Alias "WriteProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Integer, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer
    Protected Friend Declare Function WriteProcessMemory2 Lib "kernel32" Alias "WriteProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Single, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Single
    Protected Friend Declare Function WriteProcessMemory3 Lib "kernel32" Alias "WriteProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Long, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Long
    Protected Friend Declare Function ReadProcessMemory1 Lib "kernel32" Alias "ReadProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Integer, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer
    Protected Friend Declare Function ReadProcessMemory2 Lib "kernel32" Alias "ReadProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Single, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Single
    Protected Friend Declare Function ReadProcessMemory3 Lib "kernel32" Alias "ReadProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Long, ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Long
    Protected Friend Declare Function ReadProcessMemory0 Lib "kernel32" Alias "ReadProcessMemory" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByRef lpBuffer As Byte(), ByVal nSize As Integer, ByRef lpNumberOfBytesWritten As Integer) As Integer

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure MEMORY_BASIC_INFORMATION
        Public BaseAddress As IntPtr
        Public AllocationBase As IntPtr
        Public AllocationProtect As UInteger
        Public RegionSize As UIntPtr
        Public State As UInteger
        Public Protect As UInteger
        Public [Type] As UInteger
    End Structure
End Class
