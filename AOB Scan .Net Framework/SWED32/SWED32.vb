Imports System.Diagnostics
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports AOBScan.EnablePrivileges
Imports AOBScan.Memory

Namespace SWED32

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

#Region "globals"
        Private Proc As Process
#End Region
        Sub New(Process As Process)
            GoDebugPriv()
            Try
                Proc = Process
            Catch ex As Exception
                Throw New InvalidOperationException(ex.Message)
            End Try

        End Sub
        Public Sub New(ByVal procName As String)
            GoDebugPriv()
            Proc = SetProcess(procName)
        End Sub

        Sub New(ProcessPID As Integer)
            GoDebugPriv()
            Try
                Proc = Process.GetProcessById(ProcessPID)
            Catch ex As Exception
                Throw New InvalidOperationException(ex.Message)
            End Try
        End Sub
        Public Function GetProcess() As Process
            Return Proc
        End Function

        Public Function SetProcess(ByVal procName As String) As Process
            Proc = Process.GetProcessesByName(procName)(0)
            If Proc Is Nothing Then
                Throw New InvalidOperationException("Process was not found, are you using the correct bit version and have no miss-spellings?")
            End If
            Return Proc
        End Function

        Public Function GetModuleBase(ByVal moduleName As String) As IntPtr
            If String.IsNullOrEmpty(moduleName) Then
                Throw New InvalidOperationException("moduleName was either null or empty.")
            End If

            If Proc Is Nothing Then
                Throw New InvalidOperationException("process is invalid, check your init.")
            End If

            Try
                If moduleName.Contains(".exe") Then
                    If Proc.MainModule IsNot Nothing Then
                        Return Proc.MainModule.BaseAddress
                    End If
                End If

                For Each PM As ProcessModule In Proc.Modules
                    If PM.ModuleName = moduleName Then
                        Return PM.BaseAddress
                    End If
                Next
            Catch ex As Exception
                Throw New InvalidOperationException("Failed to find the specified module, search string might have miss-spellings.")
            End Try

            Return IntPtr.Zero
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr) As IntPtr
            Dim buffer(3) As Byte
            ReadProcessMemory(Proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero)
            Return CType(BitConverter.ToInt32(buffer, 0), IntPtr)
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset As Integer) As IntPtr
            Dim buffer(3) As Byte
            ReadProcessMemory(Proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero)
            Return CType(BitConverter.ToInt32(buffer, 0), IntPtr)
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offsets() As Integer) As IntPtr
            Dim buffer(3) As Byte
            For Each offset In offsets
                ReadProcessMemory(Proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero)
            Next
            Return CType(BitConverter.ToInt32(buffer, 0), IntPtr)
        End Function

#Region "ReadPointer overloads"
        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2})
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2, offset3})
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer, ByVal offset4 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2, offset3, offset4})
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer, ByVal offset4 As Integer, ByVal offset5 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2, offset3, offset4, offset5})
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer, ByVal offset4 As Integer, ByVal offset5 As Integer, ByVal offset6 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2, offset3, offset4, offset5, offset6})
        End Function

        Public Function ReadPointer(ByVal addy As IntPtr, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer, ByVal offset4 As Integer, ByVal offset5 As Integer, ByVal offset6 As Integer, ByVal offset7 As Integer) As IntPtr
            Return ReadPointer(addy, {offset1, offset2, offset3, offset4, offset5, offset6, offset7})
        End Function
#End Region

#Region "READ"
        Public Function ReadBytes(ByVal addy As IntPtr, ByVal bytes As Integer) As Byte()
            Dim buffer(bytes - 1) As Byte
            ReadProcessMemory(Proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero)
            Return buffer
        End Function

        Public Function ReadBytes(ByVal addy As IntPtr, ByVal offset As Integer, ByVal bytes As Integer) As Byte()
            Dim buffer(bytes - 1) As Byte
            ReadProcessMemory(Proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero)
            Return buffer
        End Function

        Public Function ReadInt(ByVal address As IntPtr) As Integer
            Return BitConverter.ToInt32(ReadBytes(address, 4), 0)
        End Function

        Public Function ReadInt(ByVal address As IntPtr, ByVal offset As Integer) As Integer
            Return BitConverter.ToInt32(ReadBytes(address + offset, 4), 0)
        End Function

        Public Function ReadFloat(ByVal address As IntPtr) As Single
            Return BitConverter.ToSingle(ReadBytes(address, 4), 0)
        End Function

        Public Function ReadFloat(ByVal address As IntPtr, ByVal offset As Integer) As Single
            Return BitConverter.ToSingle(ReadBytes(address + offset, 4), 0)
        End Function

        Public Function ReadVec(ByVal address As IntPtr) As Vector3
            Dim bytes = ReadBytes(address, 12)
            Return New Vector3 With {
                .X = BitConverter.ToSingle(bytes, 0),
                .Y = BitConverter.ToSingle(bytes, 4),
                .Z = BitConverter.ToSingle(bytes, 8)
            }
        End Function

        Public Function ReadVec(ByVal address As IntPtr, ByVal offset As Integer) As Vector3
            Dim bytes = ReadBytes(address + offset, 12)
            Return New Vector3 With {
                .X = BitConverter.ToSingle(bytes, 0),
                .Y = BitConverter.ToSingle(bytes, 4),
                .Z = BitConverter.ToSingle(bytes, 8)
            }
        End Function

        Public Function ReadShort(ByVal address As IntPtr) As Short
            Return BitConverter.ToInt16(ReadBytes(address, 2), 0)
        End Function

        Public Function ReadShort(ByVal address As IntPtr, ByVal offset As Integer) As Short
            Return BitConverter.ToInt16(ReadBytes(address + offset, 2), 0)
        End Function

        Public Function ReadUShort(ByVal address As IntPtr) As UShort
            Return BitConverter.ToUInt16(ReadBytes(address, 2), 0)
        End Function

        Public Function ReadUShort(ByVal address As IntPtr, ByVal offset As Integer) As UShort
            Return BitConverter.ToUInt16(ReadBytes(address + offset, 2), 0)
        End Function

        Public Function ReadUInt(ByVal address As IntPtr) As UInteger
            Return BitConverter.ToUInt32(ReadBytes(address, 4), 0)
        End Function

        Public Function ReadUInt(ByVal address As IntPtr, ByVal offset As Integer) As UInteger
            Return BitConverter.ToUInt32(ReadBytes(address + offset, 4), 0)
        End Function

        Public Function ReadMatrix(ByVal address As IntPtr) As Matrix4x4
            Dim buffer(15) As Single
            For i As Integer = 0 To 15
                buffer(i) = ReadFloat(address + (i * 4))
            Next
            Return New Matrix4x4(
                buffer(0), buffer(1), buffer(2), buffer(3),
                buffer(4), buffer(5), buffer(6), buffer(7),
                buffer(8), buffer(9), buffer(10), buffer(11),
                buffer(12), buffer(13), buffer(14), buffer(15)
            )
        End Function

        Public Function ReadMatrix(ByVal address As IntPtr, ByVal offset As Integer) As Matrix4x4
            Dim buffer(15) As Single
            For i As Integer = 0 To 15
                buffer(i) = ReadFloat(address + offset + (i * 4))
            Next
            Return New Matrix4x4(
                buffer(0), buffer(1), buffer(2), buffer(3),
                buffer(4), buffer(5), buffer(6), buffer(7),
                buffer(8), buffer(9), buffer(10), buffer(11),
                buffer(12), buffer(13), buffer(14), buffer(15)
            )
        End Function
#End Region



    End Class
End Namespace
