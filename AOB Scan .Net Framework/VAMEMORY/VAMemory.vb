Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text
Imports AOBScan.EnablePrivileges
Imports AOBScan.Memory

Namespace VAMemory

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
        Public Shared debugMode As Boolean = False
        Private mainProcess As Process()
        Private processHandle As IntPtr
        Private baseAddress As IntPtr
        Private processModule As ProcessModule

        Public ReadOnly Property getBaseAddress As Long
            Get
                Me.baseAddress = IntPtr.Zero
                Me.processModule = Me.mainProcess(0).MainModule
                Me.baseAddress = Me.processModule.BaseAddress
                Return CLng(Me.baseAddress)
            End Get
        End Property
        Sub New(Process As Process)
            GoDebugPriv()
            mainProcess(0) = Process
        End Sub
        Sub New(ProcessPID As Integer)
            GoDebugPriv()
            Me.mainProcess(0) = Process.GetProcessById(ProcessPID)
        End Sub
        Public Sub New(ByVal ProcessName As String)
            GoDebugPriv()
            ProcessName = ProcessName.Replace(".exe", "").Replace(".dll", "").Replace(".com", "")
            Me.mainProcess = Process.GetProcessesByName(ProcessName)
        End Sub

        Public Function CheckProcess() As Boolean
            If Me.mainProcess Is Nothing Then
                Console.WriteLine("Programmer, define process name first!")
                Return False
            End If

            If Me.mainProcess.Length = 0 Then
                Me.ErrorProcessNotFound(mainProcess(0).ProcessName)
                Return False
            End If

            Me.processHandle = OpenProcess(&H1F0FFF, False, Me.mainProcess(0).Id)
            If Me.processHandle = IntPtr.Zero Then
                Me.ErrorProcessNotFound(mainProcess(0).ProcessName)
                Return False
            End If



            Return True
        End Function

        Public Function ReadByteArray(ByVal pOffset As IntPtr, ByVal pSize As UInteger) As Byte()
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result() As Byte

            Try
                Dim flNewProtect As UInteger
                VirtualProtectEx(Me.processHandle, pOffset, CType(pSize, UIntPtr), &H4, flNewProtect)
                Dim array(pSize - 1) As Byte
                ReadProcessMemory(Me.processHandle, pOffset, array, pSize, 0)
                VirtualProtectEx(Me.processHandle, pOffset, CType(pSize, UIntPtr), flNewProtect, flNewProtect)
                result = array
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadByteArray" & ex.ToString())
                End If
                result = New Byte(0) {}
            End Try

            Return result
        End Function

        Public Function ReadStringUnicode(ByVal pOffset As IntPtr, ByVal pSize As UInteger) As String
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As String

            Try
                result = Encoding.Unicode.GetString(Me.ReadByteArray(pOffset, pSize), 0, CInt(pSize))
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadStringUnicode" & ex.ToString())
                End If
                result = ""
            End Try

            Return result
        End Function

        Public Function ReadStringASCII(ByVal pOffset As IntPtr, ByVal pSize As UInteger) As String
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As String

            Try
                result = Encoding.ASCII.GetString(Me.ReadByteArray(pOffset, pSize), 0, CInt(pSize))
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadStringASCII" & ex.ToString())
                End If
                result = ""
            End Try

            Return result
        End Function

        Public Function ReadChar(ByVal pOffset As IntPtr) As Char
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Char

            Try
                result = BitConverter.ToChar(Me.ReadByteArray(pOffset, 1), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadChar" & ex.ToString())
                End If
                result = " "c
            End Try

            Return result
        End Function

        Public Function ReadBoolean(ByVal pOffset As IntPtr) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                result = BitConverter.ToBoolean(Me.ReadByteArray(pOffset, 1), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadBoolean" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Public Function ReadByte(ByVal pOffset As IntPtr) As Byte
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Byte

            Try
                result = Me.ReadByteArray(pOffset, 1)(0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadByte" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadInt16(ByVal pOffset As IntPtr) As Short
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Short

            Try
                result = BitConverter.ToInt16(Me.ReadByteArray(pOffset, 2), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadInt16" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadShort(ByVal pOffset As IntPtr) As Short
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Short

            Try
                result = BitConverter.ToInt16(Me.ReadByteArray(pOffset, 2), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadShort" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadInt32(ByVal pOffset As IntPtr) As Integer
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Integer

            Try
                result = BitConverter.ToInt32(Me.ReadByteArray(pOffset, 4), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadInt32" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadInteger(ByVal pOffset As IntPtr) As Integer
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Integer

            Try
                result = BitConverter.ToInt32(Me.ReadByteArray(pOffset, 4), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadInteger" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadInt64(ByVal pOffset As IntPtr) As Long
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Long

            Try
                result = BitConverter.ToInt64(Me.ReadByteArray(pOffset, 8), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadInt64" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadLong(ByVal pOffset As IntPtr) As Long
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Long

            Try
                result = BitConverter.ToInt64(Me.ReadByteArray(pOffset, 8), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadLong" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadFloat(ByVal pOffset As IntPtr) As Single
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Single

            Try
                result = BitConverter.ToSingle(Me.ReadByteArray(pOffset, 4), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadFloat" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function ReadDouble(ByVal pOffset As IntPtr) As Double
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Double

            Try
                result = BitConverter.ToDouble(Me.ReadByteArray(pOffset, 8), 0)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: ReadDouble" & ex.ToString())
                End If
                result = 0
            End Try

            Return result
        End Function

        Public Function WriteMemory(ByVal pOffset As IntPtr, ByVal pValue As Object, ByVal pType As String) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean = False

            Try
                Select Case pType
                    Case "bytes"
                        Dim byteArray As Byte() = CType(pValue, Byte())
                        result = Me.WriteByteArray(pOffset, byteArray)
                    Case "long"
                        result = Me.WriteLong(pOffset, CType(pValue, Long))
                    Case "float"
                        result = Me.WriteFloat(pOffset, CType(pValue, Single))
                    Case "integer"
                        result = Me.WriteInteger(pOffset, CType(pValue, Integer))
                    Case "int"
                        result = Me.WriteInteger(pOffset, CType(pValue, Integer))
                    Case "double"
                        result = Me.WriteDouble(pOffset, CType(pValue, Double))
                    Case "string"
                        Dim strArray As Byte() = Encoding.UTF8.GetBytes(CStr(pValue))
                        result = Me.WriteByteArray(pOffset, strArray)
                    Case Else
                        Console.WriteLine("VAMemory WriteMemory method failed due to wrong pType provided!")
                End Select
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteMemory" & ex.ToString())
                End If
            End Try

            Return result
        End Function

        Public Function WriteByteArray(ByVal pOffset As IntPtr, ByVal byteArray As Byte()) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean = False

            Try
                Dim flNewProtect As UInteger
                VirtualProtectEx(Me.processHandle, pOffset, CType(byteArray.Length, UIntPtr), &H4, flNewProtect)
                result = WriteProcessMemory(Me.processHandle, pOffset, byteArray, CUInt(byteArray.Length), 0)
                VirtualProtectEx(Me.processHandle, pOffset, CType(byteArray.Length, UIntPtr), flNewProtect, flNewProtect)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteByteArray" & ex.ToString())
                End If
            End Try

            Return result
        End Function

        Public Function WriteInteger(ByVal pOffset As IntPtr, ByVal value As Integer) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                Dim array As Byte() = BitConverter.GetBytes(value)
                result = Me.WriteByteArray(pOffset, array)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteInteger" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Public Function WriteInt16(ByVal pOffset As IntPtr, ByVal value As Short) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                Dim array As Byte() = BitConverter.GetBytes(value)
                result = Me.WriteByteArray(pOffset, array)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteInt16" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Public Function WriteLong(ByVal pOffset As IntPtr, ByVal value As Long) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                Dim array As Byte() = BitConverter.GetBytes(value)
                result = Me.WriteByteArray(pOffset, array)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteLong" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Public Function WriteFloat(ByVal pOffset As IntPtr, ByVal value As Single) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                Dim array As Byte() = BitConverter.GetBytes(value)
                result = Me.WriteByteArray(pOffset, array)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteFloat" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Public Function WriteDouble(ByVal pOffset As IntPtr, ByVal value As Double) As Boolean
            If Me.processHandle = IntPtr.Zero Then
                Me.CheckProcess()
            End If

            Dim result As Boolean

            Try
                Dim array As Byte() = BitConverter.GetBytes(value)
                result = Me.WriteByteArray(pOffset, array)
            Catch ex As Exception
                If debugMode Then
                    Console.WriteLine("Error: WriteDouble" & ex.ToString())
                End If
                result = False
            End Try

            Return result
        End Function

        Private Sub ErrorProcessNotFound(ByVal processName As String)
            Debug.WriteLine("Process " & processName & " not found!", "VAMemory Class")
        End Sub

        Public Function GetDLL(ByVal name As String) As Long
            Dim DLL As Long = 0
            If CheckProcess() Then
                For Each m As ProcessModule In mainProcess(0).Modules
                    If m.ModuleName = name Then
                        DLL = CLng(m.BaseAddress)
                        Return DLL
                    End If
                Next
            End If
            Return DLL
        End Function

        Public Function MultiPointer(ByVal BaseAddr As Long, ByVal BaseOffset As Integer, ByVal Pointers() As Integer) As Integer
            Dim Current As Integer = ReadInt32(CInt(BaseAddr + BaseOffset))

            For i As Integer = 0 To Pointers.Length - 2
                Current = ReadInt32(Current + Pointers(i))
            Next

            Return Current + Pointers(Pointers.Length - 1)
        End Function

        Public Function MultiPointer64(ByVal BaseAddr As Long, ByVal BaseOffset As Integer, ByVal Pointers() As Integer) As Long
            If CheckProcess() Then
                Dim Current As Long = ReadInt64(BaseAddr + BaseOffset)

                For i As Integer = 0 To Pointers.Length - 2
                    Current = ReadInt64(Current + Pointers(i))
                Next

                Return Current + Pointers(Pointers.Length - 1)
            End If
            Return 0
        End Function



    End Class
End Namespace

