Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports AOBScan.EnablePrivileges
Imports AOBScan.Memory


Namespace CheatEngine

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
    Public Class Processor
        Private Property MyProcess As Process

        Sub New(ByVal ProcessName As String)
            Call GoDebugPriv()
            MyProcess = New Process
            MyProcess = Process.GetProcessesByName(RemoveExtension(ProcessName)).FirstOrDefault()
        End Sub
        Sub New(ByVal ProcessPID As Integer)
            Call GoDebugPriv()
            MyProcess = New Process
            MyProcess = Process.GetProcessById(ProcessPID)
        End Sub


#Region "AOB Scan"
        ' Função para realizar o AOB scan com entrada em Byte array
        Public Function AOBScanGetAddressByte(ByVal PatternBytes As String) As Byte()
            Dim Result As IntPtr = AOBScanInProcess(PatternBytes)
            If IntPtr.Size = 4 Then
                ' Para 32 bits, converte para 4 bytes
                Return BitConverter.GetBytes(Result.ToInt32())
            Else
                ' Para 64 bits, converte para 8 bytes
                Return BitConverter.GetBytes(Result.ToInt64())
            End If
        End Function
        ' Função para obter o endereço da memória como String
        Public Function AOBScanGetAddressStr(ByVal PatternString As String) As String
            Return AOBScanInProcess(PatternString).ToString("X")
        End Function
        ' Função para obter o endereço da memória como intptr
        Public Function AOBScanGetAddressIntPtr(ByVal PatternString As String) As IntPtr
            Return AOBScanInProcess(PatternString)
        End Function

#Region "ASYNC METHOD"
        Public Async Function AOBScanStrAsync(ByVal PatternBytes As String) As Task(Of String)
            Return Await Task.Run(Of String)(Function()
                                                 Return AOBScanGetAddressStr(PatternBytes)
                                             End Function)
        End Function
        Public Async Function AOBScanIntPtrAsync(ByVal PatternString As String) As Task(Of IntPtr)
            Return Await Task.Run(Of IntPtr)(Function()
                                                 Return AOBScanGetAddressIntPtr(PatternString)
                                             End Function)
        End Function
        Public Async Function AOBScanByteAsync(ByVal PatternString As String) As Task(Of Byte())
            Return Await Task.Run(Of Byte())(Function()
                                                 Return AOBScanGetAddressByte(PatternString)
                                             End Function)
        End Function

#End Region

#End Region

#Region "Scan AOB List"
        ' Função que realiza o escaneamento e retorna uma lista de endereços encontrados
        Public Function AOBScanListStr(ByVal pattern As String) As List(Of String)
            Return AOBScanListTratament(pattern).Select(Function(ptr) ptr.ToString("X")).ToList()
        End Function
        Public Function AOBScanListIntPtr(ByVal Pattern As String) As List(Of IntPtr)
            Return AOBScanListTratament(Pattern)
        End Function
        Public Function AOBScanListByte(ByVal Pattern As String) As List(Of Byte)
            Dim byteList As New List(Of Byte)()
            For Each ptr As IntPtr In AOBScanListTratament(Pattern)
                ' Converte o IntPtr para um array de bytes
                Dim bytes As Byte() = BitConverter.GetBytes(ptr.ToInt64())
                byteList.AddRange(bytes)
            Next
            Return byteList
        End Function
        Public Function AOBScanListByte(ByVal Pattern As Byte()) As List(Of Byte)
            Dim byteList As New List(Of Byte)()
            For Each ptr As IntPtr In AOBScanListTratament(BitConverter.ToString(Pattern))
                ' Converte o IntPtr para um array de bytes
                Dim bytes As Byte() = BitConverter.GetBytes(ptr.ToInt64())
                byteList.AddRange(bytes)
            Next
            Return byteList
        End Function


#Region "ASYNC METHOD"
        ' Função que realiza o escaneamento e retorna uma lista de endereços encontrados
        Public Async Function AOBScanListStrAsync(ByVal pattern As String) As Task(Of List(Of String))
            Return Await Task.Run(Of List(Of String))(Function()
                                                          Return AOBScanListStr(pattern)
                                                      End Function)
        End Function
        Public Async Function AOBScanListIntPtrAsync(ByVal Pattern As String) As Task(Of List(Of IntPtr))
            Return Await Task.Run(Of List(Of IntPtr))(Function()
                                                          Return AOBScanListIntPtr(Pattern)
                                                      End Function)
        End Function
        Public Async Function AOBScanListByteAsync(ByVal Pattern As String) As Task(Of List(Of Byte))
            Return Await Task.Run(Of List(Of Byte))(Function()
                                                        Return AOBScanListByte(Pattern)
                                                    End Function)
        End Function

#End Region
#End Region


#Region "Replace Bytes Of Array"

        ' Função para substituir bytes na memória
        Public Function ReplaceBytesIntPtr(ByVal SearchByte As String, ByVal ReplaceByte As String) As List(Of IntPtr)
            Try

                ' Verificar se o tamanho dos arrays de busca e substituição são iguais
                If SearchByte.Length <> ReplaceByte.Length Then
                    Console.WriteLine("Tamanhos dos arrays de bytes não coincidem")
                    Return Nothing
                End If

                Dim ListAddressesWrited As New List(Of IntPtr)
                Dim Search As Byte() = ConvertPatternToBytes(SearchByte)
                Dim Replace As Byte() = ConvertPatternToBytes(ReplaceByte)
                Dim BytesWritten As UInteger = 0

                For Each AddressToWrite As IntPtr In AOBScanListIntPtr(SearchByte)
                    If WriteProcessMemory(MyProcess.Handle, AddressToWrite, Replace, CUInt(ReplaceByte.Length), BytesWritten) Then
                        ListAddressesWrited.Add(AddressToWrite)
                    End If
                Next

                Return ListAddressesWrited

            Catch ex As Exception
                Console.WriteLine("Erro: " & ex.Message)
                Return Nothing
            End Try
        End Function
        Public Function ReplaceBytesStr(ByVal SearchByte As String, ByVal ReplaceByte As String) As List(Of String)
            Try

                ' Verificar se o tamanho dos arrays de busca e substituição são iguais
                If SearchByte.Length <> ReplaceByte.Length Then
                    Console.WriteLine("Tamanhos dos arrays de bytes não coincidem")
                    Return Nothing
                End If

                Dim ListAddressesWrited As New List(Of String)
                Dim Search As Byte() = ConvertPatternToBytes(SearchByte)
                Dim Replace As Byte() = ConvertPatternToBytes(ReplaceByte)
                Dim BytesWritten As UInteger = 0

                For Each AddressToWrite As IntPtr In AOBScanListIntPtr(SearchByte)
                    If WriteProcessMemory(MyProcess.Handle, AddressToWrite, Replace, CUInt(ReplaceByte.Length), BytesWritten) Then
                        ListAddressesWrited.Add(AddressToWrite.ToString("X"))
                    End If
                Next

                Return ListAddressesWrited

            Catch ex As Exception
                Console.WriteLine("Erro: " & ex.Message)
                Return Nothing
            End Try
        End Function
        Public Function ReplaceBytesByte(ByVal SearchByte As String, ByVal ReplaceByte As String) As List(Of Byte)
            Try

                ' Verificar se o tamanho dos arrays de busca e substituição são iguais
                If SearchByte.Length <> ReplaceByte.Length Then
                    Console.WriteLine("Tamanhos dos arrays de bytes não coincidem")
                    Return Nothing
                End If

                Dim ListAddressesWrited As New List(Of IntPtr)
                Dim Search As Byte() = ConvertPatternToBytes(SearchByte)
                Dim Replace As Byte() = ConvertPatternToBytes(ReplaceByte)
                Dim BytesWritten As UInteger = 0

                For Each AddressToWrite As IntPtr In AOBScanListIntPtr(SearchByte)
                    If WriteProcessMemory(MyProcess.Handle, AddressToWrite, Replace, CUInt(ReplaceByte.Length), BytesWritten) Then
                        ListAddressesWrited.Add(AddressToWrite)
                    End If
                Next

                Dim ByteList As New List(Of Byte)()
                For Each ptr As IntPtr In ListAddressesWrited
                    ' Converte o IntPtr para um array de bytes
                    ByteList.AddRange(BitConverter.GetBytes(ptr.ToInt64()))
                Next

                Return ByteList


            Catch ex As Exception
                Console.WriteLine("Erro: " & ex.Message)
                Return Nothing
            End Try
        End Function
        Public Function ReplaceBytesByte(ByVal SearchByte As Byte(), ByVal ReplaceByte As Byte()) As List(Of Byte)
            Try

                ' Verificar se o tamanho dos arrays de busca e substituição são iguais
                If SearchByte.Length <> ReplaceByte.Length Then
                    Console.WriteLine("Tamanhos dos arrays de bytes não coincidem")
                    Return Nothing
                End If

                Dim ListAddressesWrited As New List(Of IntPtr)
                Dim BytesWritten As UInteger = 0

                For Each AddressToWrite As IntPtr In AOBScanListByte(SearchByte)
                    If WriteProcessMemory(MyProcess.Handle, AddressToWrite, ReplaceByte, CUInt(ReplaceByte.Length), BytesWritten) Then
                        ListAddressesWrited.Add(AddressToWrite)
                    End If
                Next

                Dim ByteList As New List(Of Byte)()
                For Each ptr As IntPtr In ListAddressesWrited
                    ' Converte o IntPtr para um array de bytes
                    ByteList.AddRange(BitConverter.GetBytes(ptr.ToInt64()))
                Next

                Return ByteList

            Catch ex As Exception
                Console.WriteLine("Erro: " & ex.Message)
                Return Nothing
            End Try
        End Function

#Region "ASYNC METHOD"
        Public Async Function ReplaceBytesIntPtrAsync(ByVal SearchByte As String, ByVal ReplaceByte As String) As Task(Of List(Of IntPtr))
            Return Await Task.Run(Of List(Of IntPtr))(Function()
                                                          Return ReplaceBytesIntPtr(SearchByte, ReplaceByte)
                                                      End Function)
        End Function
        Public Async Function ReplaceBytesStrAsync(ByVal SearchByte As String, ByVal ReplaceByte As String) As Task(Of List(Of String))
            Return Await Task.Run(Of List(Of String))(Function()
                                                          Return ReplaceBytesStr(SearchByte, ReplaceByte)
                                                      End Function)
        End Function
        Public Async Function ReplaceBytesByteAsync(ByVal SearchByte As String, ByVal ReplaceByte As String) As Task(Of List(Of Byte))
            Return Await Task.Run(Of List(Of Byte))(Function()
                                                        Return ReplaceBytesByte(SearchByte, ReplaceByte)
                                                    End Function)
        End Function
#End Region


#End Region


#Region "Read Region"

#Region "STRING"
        ' Função que retorna um valor como String
        Public Function CEReadMemoryStr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = Encoding.UTF8.GetString(buffer).TrimEnd(Chr(0))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryStr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = Encoding.UTF8.GetString(buffer).TrimEnd(Chr(0))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryStr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next

                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = Encoding.UTF8.GetString(buffer).TrimEnd(Chr(0))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryStr(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next

                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = Encoding.UTF8.GetString(buffer).TrimEnd(Chr(0))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryStr(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = Encoding.UTF8.GetString(buffer).TrimEnd(Chr(0))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function ReadMemoryStr(ByVal AddressString As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String
            Try

                ' Verifica se o processo está disponível
                If MyProcess Is Nothing Then Return String.Empty

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressString) = False Then
                    Return Nothing
                End If

                ' Converte o endereço inteiro para IntPtr
                Dim AddressIntPtr As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))
                AddressIntPtr += Jump

                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                ' Lê os bytes da memória e verifica se a leitura foi bem-sucedida
                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    ' Converte o buffer de bytes para string usando codificação ASCII
                    Return System.Text.Encoding.ASCII.GetString(buffer)
                End If

                ' Retorna uma string vazia em caso de falha
                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function ReadMemoryStr(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String

            Try

                ' Verifica se o processo está disponível
                If MyProcess Is Nothing Then Return String.Empty

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressIntPtr) = False Then
                    Return Nothing
                End If

                AddressIntPtr += Jump

                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                ' Lê os bytes da memória e verifica se a leitura foi bem-sucedida
                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)

                    ' Converte o buffer de bytes para string usando codificação ASCII
                    Return System.Text.Encoding.ASCII.GetString(buffer)
                End If

                ' Retorna uma string vazia em caso de falha
                Return String.Empty
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function ReadMemoryStr(ByVal AddressInt As Integer, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As String

            Try

                ' Verifica se o processo está disponível
                If MyProcess Is Nothing Then Return String.Empty

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressInt) = False Then
                    Return Nothing
                End If

                AddressInt += Jump

                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                ' Lê os bytes da memória e verifica se a leitura foi bem-sucedida
                If ReadProcessMemory(MyProcess.Handle, AddressInt, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)

                    ' Converte o buffer de bytes para string usando codificação ASCII
                    Return System.Text.Encoding.ASCII.GetString(buffer)
                End If

                ' Retorna uma string vazia em caso de falha
                Return String.Empty
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

#End Region

#Region "INTEGER"

        Public Function CEReadMemoryInt(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Integer = BitConverter.ToInt32(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function CEReadMemoryInt(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Integer = BitConverter.ToInt32(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryInt(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Integer = BitConverter.ToInt32(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function CEReadMemoryInt(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Integer = BitConverter.ToInt32(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryInt(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Integer = BitConverter.ToInt32(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadMemoryInt(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer

            Try
                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressIntPtr) = False Then
                    Return Nothing
                End If

                AddressIntPtr += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0
                Dim BitesForRead As Byte() = Nothing

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, ByteCount, 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, ByteCount, flNewProtect, flNewProtect)
                    Return BitConverter.ToUInt64(buffer)
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function ReadMemoryInt(ByVal AddressString As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressString) = False Then
                    Return Nothing
                End If

                ' Converte o endereço inteiro para IntPtr
                Dim AddressIntPtr As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))
                AddressIntPtr += Jump

                ' Buffer para armazenar os bytes lidos da memória
                Dim buffer(ByteCount - 1) As Byte

                ' Função que faz a leitura de memória
                'Marshal.Copy(address, buffer, 0, ByteCount)


                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, ByteCount, 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, ByteCount, flNewProtect, flNewProtect)
                    Return Convert.ToUInt64(buffer(0)) ' 1 byte
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function ReadMemoryInt(ByVal AddressInt As Integer, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Integer

            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(New IntPtr(AddressInt)) = False Then
                    Return Nothing
                End If

                AddressInt += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressInt, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return BitConverter.ToUInt64(buffer)
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function

#End Region

#Region "DOUBLE"
        Public Function CEReadMemoryDouble(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function CEReadMemoryDouble(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function CEReadMemoryDouble(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryDouble(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function CEReadMemoryDouble(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function ReadMemoryDouble(ByVal AddressString As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressString) = False Then
                    Return Nothing
                End If

                ' Converte o endereço inteiro para IntPtr
                Dim AddressIntPtr As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))
                AddressIntPtr += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim DoubleValue As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    Return DoubleValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadMemoryDouble(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressIntPtr) = False Then
                    Return Nothing
                End If

                AddressIntPtr += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim DoubleValue As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    Return DoubleValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function ReadMemoryDouble(ByVal AddressInt As Integer, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Double
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(New IntPtr(AddressInt)) = False Then
                    Return Nothing
                End If

                AddressInt += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressInt, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim DoubleValue As Double = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(buffer))
                    Return DoubleValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function


#End Region

#Region "FLOAT"

        Public Function CEReadMemoryFloat(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function CEReadMemoryFloat(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function CEReadMemoryFloat(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim FloatValue As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return FloatValue
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryFloat(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim FloatValue As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return FloatValue
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryFloat(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadMemoryFloat(ByVal AddressString As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressString) = False Then
                    Return Nothing
                End If

                ' Converte o endereço inteiro para IntPtr
                Dim AddressIntPtr As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))
                AddressIntPtr += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim FloatValue As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    Return FloatValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function ReadMemoryFloat(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(AddressIntPtr) = False Then
                    Return Nothing
                End If

                AddressIntPtr += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim FloatValue As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))
                    Return FloatValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function
        Public Function ReadMemoryFloat(ByVal AddressInt As Integer, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Single
            Try

                If MyProcess Is Nothing Then Return Nothing

                'VERIFICAÇÃO DE PATTERN
                If VerificationPattern(New IntPtr(AddressInt)) = False Then
                    Return Nothing
                End If

                AddressInt += Jump

                Dim buffer As Byte() = New Byte(ByteCount - 1) {}
                Dim bytesRead As Integer = 0

                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, AddressInt, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Dim FloatValue As Single = BitConverter.Int32BitsToSingle(BitConverter.ToInt32(buffer))

                    Return FloatValue
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try

        End Function

#End Region

#Region "BYTE"

        Public Function CEReadMemoryBytes(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    'Dim Result As Byte = buffer
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return buffer
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryBytes(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return buffer
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryBytes(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return buffer
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryBytes(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return buffer
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryBytes(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return buffer
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Function ReadMemoryByte(ByVal AddressStr As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte

            If MyProcess Is Nothing Then Return Nothing

            Dim AddressIntPtr As IntPtr = New IntPtr(Convert.ToInt64(AddressStr, 16))
            Dim buffer As Byte() = New Byte(ByteCount - 1) {}
            Dim bytesRead As Integer = 0

            AddressIntPtr += Jump

            Dim flNewProtect As UInteger

            ' Altera a proteção de memória da página para permitir leitura
            VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


            If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                ' Restaura a proteção de memória original
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                Return BitConverter.ToInt32(buffer)
            End If

            Return Nothing
        End Function
        Public Function ReadMemoryByte(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte
            If MyProcess Is Nothing Then Return Nothing
            Dim buffer As Byte() = New Byte(ByteCount - 1) {}
            Dim bytesRead As Integer = 0
            Dim flNewProtect As UInteger

            'VERIFICAÇÃO DE PATTERN
            If VerificationPattern(AddressIntPtr) = False Then
                Return Nothing
            End If

            AddressIntPtr += Jump

            ' Altera a proteção de memória da página para permitir leitura
            VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


            If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                ' Restaura a proteção de memória original
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                Return Convert.ToInt32(buffer)
            End If

            Return Nothing
        End Function
        Public Function ReadMemoryByte(ByVal AddressInt As Integer, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            If MyProcess Is Nothing Then Return Nothing
            Dim buffer As Byte() = New Byte(ByteCount - 1) {}
            Dim bytesRead As Integer = 0
            Dim flNewProtect As UInteger

            'VERIFICAÇÃO DE PATTERN
            If VerificationPattern(New IntPtr(AddressInt)) = False Then
                Return Nothing
            End If

            AddressInt += Jump

            ' Altera a proteção de memória da página para permitir leitura
            VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


            If ReadProcessMemory(MyProcess.Handle, AddressInt, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                ' Restaura a proteção de memória original
                VirtualProtectEx(MyProcess.Handle, AddressInt, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                Return buffer
            End If

            Return Nothing
        End Function
        Public Function ReadMemoryBytes(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As Byte()
            If MyProcess Is Nothing Then Return Nothing
            Dim buffer As Byte() = New Byte(ByteCount - 1) {}
            Dim bytesRead As Integer = 0
            Dim flNewProtect As UInteger

            'VERIFICAÇÃO DE PATTERN
            If VerificationPattern(AddressIntPtr) = False Then
                Return Nothing
            End If

            AddressIntPtr += Jump

            ' Altera a proteção de memória da página para permitir leitura
            VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


            If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                ' Restaura a proteção de memória original
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                Return buffer
            End If

            Return Nothing
        End Function
        Private Function ReadMemoryPointer(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr
            If MyProcess Is Nothing Then Return Nothing
            Dim buffer As Byte() = New Byte(ByteCount - 1) {}
            Dim bytesRead As Integer = 0
            Dim flNewProtect As UInteger

            'VERIFICAÇÃO DE PATTERN
            If VerificationPattern(AddressIntPtr) = False Then
                Return Nothing
            End If

            AddressIntPtr += Jump

            ' Altera a proteção de memória da página para permitir leitura
            VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


            If ReadProcessMemory(MyProcess.Handle, AddressIntPtr, buffer, ByteCount, bytesRead) AndAlso bytesRead = ByteCount Then
                ' Restaura a proteção de memória original
                VirtualProtectEx(MyProcess.Handle, AddressIntPtr, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                Return New IntPtr(BitConverter.ToInt32(buffer))
            End If

            Return Nothing
        End Function


#End Region

#Region "INTPTR"
        Public Function CEReadMemoryIntPtr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr()
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 8)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As IntPtr() = ByteArrayToIntPtrArray(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryIntPtr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr()
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As IntPtr() = ByteArrayToIntPtrArray(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryIntPtr(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr()
            Try

                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As IntPtr() = ByteArrayToIntPtrArray(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryIntPtr(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr()
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As IntPtr() = ByteArrayToIntPtrArray(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryIntPtr(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0) As IntPtr()
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As IntPtr() = ByteArrayToIntPtrArray(buffer)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryGetAddress(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, Optional ByVal Jump As Integer = &H0) As IntPtr
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 8)
                    End If
                Next


                Return SumPointer

            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryGetAddress(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), Optional ByVal Jump As Integer = &H0) As IntPtr
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next



                Return SumPointer

            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryGetAddress(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), Optional ByVal Jump As Integer = &H0) As IntPtr
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next

                Return SumPointer

            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryGetAddress(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String, Optional ByVal Jump As Integer = &H0) As IntPtr
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next

                Return SumPointer

            Catch ex As Exception
                Return Nothing
            End Try
        End Function

#End Region

#Region "HEX"
        Public Function CEReadMemoryHex(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String, ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0, Optional ByVal Split As String = "-") As String
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets As String() = Nothing

                If CheatEngineOffSet.Contains(",") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split(",")
                ElseIf CheatEngineOffSet.Contains("-") Then
                    OffSets = CheatEngineOffSet.Replace(" ", "").Split("-")
                Else
                    OffSets = CheatEngineOffSet.Split(" ")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryInt(SumPointer, 8)



                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 8)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = BitConverter.ToString(buffer).Replace("-", Split)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryHex(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0, Optional ByVal Split As String = "-") As String
            Try
                Dim CheatTratament As String() = Nothing
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = BitConverter.ToString(buffer).Replace("-", Split)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryHex(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As String(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0, Optional ByVal Split As String = "-") As String
            Try
                Dim OffSets(CheatEngineOffSet.Count - 1) As String

                For i = 0 To CheatEngineOffSet.Count - 1
                    If CheatEngineOffSet(i).Contains(",") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(",", "")
                    ElseIf CheatEngineOffSet(i).Contains("-") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace("-", "")
                    ElseIf CheatEngineOffSet(i).Contains(" ") Then
                        OffSets(i) = CheatEngineOffSet(i).Replace(" ", "")
                    Else
                        OffSets(i) = CheatEngineOffSet(i)
                    End If
                Next

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To OffSets.Count - 1
                    If VerificationPattern(OffSets(i)) = False Then
                        Return Nothing
                    End If
                Next


                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = CheatEnginePointer
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To OffSets.Count - 1
                    If i >= OffSets.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, Convert.ToInt32(OffSets(i), 16))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)

                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = BitConverter.ToString(buffer).Replace("-", Split)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If


                Return Nothing
            Catch ex As Exception

                Console.WriteLine(ex.Message)
                Return Nothing
            End Try

        End Function
        Public Function CEReadMemoryHex(ByVal CheatEnginePointer As String, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0, Optional ByVal Split As String = "-") As String
            Try
                Dim CheatTratament As String() = Nothing

                If CheatEnginePointer.Contains("+") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("+")
                ElseIf CheatEnginePointer.Contains("-") Then
                    CheatTratament = CheatEnginePointer.Replace(" ", "").Split("-")
                End If

                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim ProcessCheat = Process.GetProcessesByName(RemoveExtension(CheatTratament(0))).FirstOrDefault()
                Dim BaseAddress As IntPtr = ProcessCheat.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(Convert.ToInt32(CheatTratament(1).TrimStart("0"c), 16))
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = BitConverter.ToString(buffer).Replace("-", Split)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
        Public Function CEReadMemoryHex(ByVal CheatEnginePointer As Integer, ByVal CheatEngineOffSet As Integer(), ByVal ByteCount As Integer, Optional ByVal Jump As Integer = &H0, Optional ByVal Split As String = "-") As String
            Try
                Dim CheatTratament As String() = Nothing


                'VERIFICAÇÃO DE PATTERN
                For i = 0 To CheatEngineOffSet.Count - 1
                    If VerificationPattern(CheatEngineOffSet(i)) = False Then
                        Return Nothing
                    End If
                Next

                Dim BaseAddress As IntPtr = MyProcess.MainModule.BaseAddress
                Dim Pointer As IntPtr = New IntPtr(CheatEnginePointer)
                Dim SumPointer As IntPtr = IntPtr.Add(BaseAddress, Pointer)
                SumPointer = ReadMemoryPointer(SumPointer, 16)


                For i = 0 To CheatEngineOffSet.Count - 1
                    If i >= CheatEngineOffSet.Count - 1 Then
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer += Jump
                    Else
                        SumPointer = IntPtr.Add(SumPointer, CheatEngineOffSet(i))
                        SumPointer = ReadMemoryPointer(SumPointer, 16)
                    End If
                Next


                ' Cria um buffer para armazenar os bytes lidos
                Dim buffer(ByteCount - 1) As Byte
                Dim bytesRead As Integer = 0
                Dim flNewProtect As UInteger

                ' Altera a proteção de memória da página para permitir leitura
                VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), 4UI, flNewProtect)


                If ReadProcessMemory(MyProcess.Handle, SumPointer, buffer, buffer.Length, bytesRead) Then
                    Dim Result As String = BitConverter.ToString(buffer).Replace("-", Split)
                    ' Restaura a proteção de memória original
                    VirtualProtectEx(MyProcess.Handle, SumPointer, CType(ByteCount, UIntPtr), flNewProtect, flNewProtect)
                    Return Result
                End If

                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

#End Region




#Region "ASYNC METHOD"
        Public Async Function ReadMemoryStrAsync(ByVal AddressString As String, ByVal ByteCount As Integer) As Task(Of String)
            Return Await Task.Run(Of String)(Function()
                                                 Return ReadMemoryStr(AddressString, ByteCount)
                                             End Function)
        End Function
        Public Async Function ReadMemoryStrAsync(ByVal AddressByte As IntPtr, ByVal ByteCount As Integer) As Task(Of String)
            Return Await Task.Run(Of String)(Function()
                                                 Return ReadMemoryStr(AddressByte, ByteCount)
                                             End Function)
        End Function
        Public Async Function ReadMemoryIntAsync(ByVal AddressInteger As String, ByVal ByteCount As Integer) As Task(Of Integer)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryInt(AddressInteger, ByteCount)
                                              End Function)
        End Function
        Public Async Function ReadMemoryIntAsync(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer) As Task(Of Integer)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryInt(AddressIntPtr, ByteCount)
                                              End Function)
        End Function
        Public Async Function ReadMemoryDoubleAsync(ByVal AddressInteger As String, ByVal ByteCount As Integer) As Task(Of Double)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryDouble(AddressInteger, ByteCount)
                                              End Function)
        End Function
        Public Async Function ReadMemoryDoubleAsync(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer) As Task(Of Double)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryDouble(AddressIntPtr, ByteCount)
                                              End Function)
        End Function

        Public Async Function ReadMemoryFloatAsync(ByVal AddressInteger As String, ByVal ByteCount As Integer) As Task(Of Single)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryFloat(AddressInteger, ByteCount)
                                              End Function)
        End Function
        Public Async Function ReadMemoryFloatAsync(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer) As Task(Of Single)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryFloat(AddressIntPtr, ByteCount)
                                              End Function)
        End Function

        Public Async Function ReadMemoryByteAsync(ByVal AddressInteger As String, ByVal ByteCount As Integer) As Task(Of Byte)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryByte(AddressInteger, ByteCount)
                                              End Function)
        End Function
        Public Async Function ReadMemoryByteAsync(ByVal AddressIntPtr As IntPtr, ByVal ByteCount As Integer) As Task(Of Byte)
            Return Await Task.Run(Of Integer)(Function()
                                                  Return ReadMemoryByte(AddressIntPtr, ByteCount)
                                              End Function)
        End Function
#End Region

#End Region

#Region "Write Region"
        Public Function WriteMemory(ByVal AddressString As String, ByVal NewValue As String) As Boolean

            If VerificationPattern(AddressString) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o endereço da memória de string hexadecimal para IntPtr
            Dim address As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(NewValue)
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, address, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function

        Public Function WriteMemory(ByVal AddressString As String, ByVal NewValue As Integer) As Boolean
            If VerificationPattern(AddressString) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o endereço da memória de string hexadecimal para IntPtr
            Dim address As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))

            ' Converte o valor inteiro para um array de bytes
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue)
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, address, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function

        Public Function WriteMemory(ByVal AddressString As String, ByVal NewValue As Byte()) As Boolean

            If VerificationPattern(AddressString) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Converte o endereço da memória de string hexadecimal para IntPtr
            Dim address As IntPtr = New IntPtr(Convert.ToInt64(AddressString, 16))

            'Dim process As Process = process.GetProcessesByName(processName).FirstOrDefault()
            If MyProcess Is Nothing Then Return False

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, address, NewValue, NewValue.Length, bytesWritten) AndAlso bytesWritten = NewValue.Length
        End Function

        Public Function WriteMemory(ByVal AddressIntPtr As Integer, ByVal NewValue As Byte()) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, NewValue, NewValue.Length, bytesWritten) AndAlso bytesWritten = NewValue.Length
        End Function

        Public Function WriteMemory(ByVal AddressIntPtr As Integer, ByVal NewValue As String) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(NewValue)

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntPtr As Integer, ByVal NewValue As Double) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(NewValue)

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntPtr As Integer, ByVal NewValue As Single) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(NewValue)

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntPtr As IntPtr, ByVal NewValue As Integer) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o valor inteiro para um array de bytes
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue)
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntPtr As IntPtr, ByVal NewValue As Double) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o valor inteiro para um array de bytes
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue)
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntPtr As IntPtr, ByVal NewValue As Single) As Boolean
            If VerificationPattern(AddressIntPtr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o valor inteiro para um array de bytes
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue)
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, AddressIntPtr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressIntptr As IntPtr, ByVal NewValue As IntPtr) As Boolean
            If VerificationPattern(AddressIntptr) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            ' Verifica se o processo está disponível
            If MyProcess Is Nothing Then Return False

            ' Converte o valor inteiro para um array de bytes
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue.ToInt32())
            Dim bytesWritten As Integer = 0

            ' Escreve os bytes na memória do processo e verifica se a operação foi bem-sucedida
            Return WriteProcessMemory(MyProcess.Handle, AddressIntptr, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function
        Public Function WriteMemory(ByVal AddressByte As Byte(), ByVal NewValue As String) As Boolean
            If VerificationPattern(AddressByte.ToString("X")) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            'Converte o endereço da memória de string hexadecimal para IntPtr
            Dim address As IntPtr = BitConverter.ToInt32(AddressByte)

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(NewValue)

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, address, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function

        Public Function WriteMemory(ByVal AddressByte As Byte(), ByVal NewValue As Integer) As Boolean
            If VerificationPattern(AddressByte.ToString("X")) = False Then
                Console.WriteLine("Wrong Address.")
                Return False
            End If

            If MyProcess Is Nothing Then Return False

            'Converte o endereço da memória de string hexadecimal para IntPtr
            Dim address As IntPtr = BitConverter.ToInt32(AddressByte)

            ' Converte o novo valor de string para um array de bytes usando UTF-8
            Dim bytes As Byte() = BitConverter.GetBytes(NewValue)

            Dim bytesWritten As Integer = 0
            Return WriteProcessMemory(MyProcess.Handle, address, bytes, bytes.Length, bytesWritten) AndAlso bytesWritten = bytes.Length
        End Function

#Region "ASYNC METHOD"
        Public Async Function WriteMemoryAsync(ByVal AddressString As String, ByVal NewValue As String) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressString, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressString As String, ByVal NewValue As Integer) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressString, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressString As String, ByVal NewValue As Byte()) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressString, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressIntPtr As Integer, ByVal NewValue As Byte()) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressIntPtr, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressIntPtr As Integer, ByVal NewValue As String) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressIntPtr, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressByte As Byte(), ByVal NewValue As String) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressByte, NewValue)
                                              End Function)
        End Function
        Public Async Function WriteMemoryAsync(ByVal AddressByte As Byte(), ByVal NewValue As Integer) As Task(Of Boolean)
            Return Await Task.Run(Of Boolean)(Function()
                                                  Return WriteMemory(AddressByte, NewValue)
                                              End Function)
        End Function


#End Region




#End Region

#Region "Functions Tools"

        ' Função que faz a varredura da memória com suporte a curingas
        Private Function AOBScanInProcess(ByVal pattern As String) As IntPtr
            If VerificationPattern(pattern) = False Then
                Return Nothing
            End If

            Dim PatternBytes As Byte() = ConvertPatternToBytes(pattern)

            If MyProcess Is Nothing Then
                Console.WriteLine("Processo não encontrado.")
                Return IntPtr.Zero
            End If

            Const PROCESS_VM_READ As Integer = &H10
            Const PROCESS_QUERY_INFORMATION As Integer = &H400

            Dim byteList As New List(Of Byte)
            Dim wildcards As New List(Of Integer)

            ' Divide a string em pares de dois dígitos
            Dim hexParts As String() = pattern.Split(" "c)


            For i As Integer = 0 To hexParts.Length - 1
                If hexParts(i) = "??" OrElse hexParts(i) = "XX" Then
                    ' Marca a posição do curinga
                    byteList.Add(0) ' Adiciona um valor placeholder, mas não será comparado
                    wildcards.Add(i) ' Adiciona a posição do curinga
                Else
                    ' Converte o par de dígitos hexadecimais em byte
                    byteList.Add(Convert.ToByte(hexParts(i), 16))
                End If
            Next

            Dim PatternResult As Byte() = byteList.ToArray()

            ' Abre o processo com permissões necessárias
            Dim processHandle As IntPtr = OpenProcess(PROCESS_VM_READ Or PROCESS_QUERY_INFORMATION, False, MyProcess.Id)
            If processHandle = IntPtr.Zero Then
                Throw New Exception("Não foi possível abrir o processo.")
            End If

            Dim memoryInfo As New MEMORY_BASIC_INFORMATION()
            Dim baseAddress As IntPtr = IntPtr.Zero

            ' Varre a memória do processo
            While VirtualQueryEx(processHandle, baseAddress, memoryInfo, Marshal.SizeOf(memoryInfo)) > 0
                If memoryInfo.State = &H1000 AndAlso (memoryInfo.Protect = &H4 OrElse memoryInfo.Protect = &H20) Then ' Verifica se a memória está acessível
                    Dim buffer(memoryInfo.RegionSize - 1) As Byte
                    Dim bytesRead As Integer

                    ' Lê o bloco de memória
                    If ReadProcessMemory(processHandle, baseAddress, buffer, buffer.Length, bytesRead) Then
                        ' Verifica se o padrão de bytes está presente nesse bloco
                        For i As Integer = 0 To bytesRead - PatternResult.Length
                            Dim found As Boolean = True
                            For j As Integer = 0 To PatternResult.Length - 1
                                ' Ignora as posições com wildcards durante a comparação
                                If Not wildcards.Contains(j) AndAlso buffer(i + j) <> PatternResult(j) Then
                                    found = False
                                    Exit For
                                End If
                            Next

                            If found Then
                                Return baseAddress + i
                            End If
                        Next
                    End If
                End If
                ' Atualiza o endereço base para a próxima região de memória
                baseAddress = IntPtr.Add(baseAddress.ToInt64(), memoryInfo.RegionSize)
            End While

            Return IntPtr.Zero
        End Function
        Public Function AOBScanListTratament(ByVal pattern As String) As List(Of IntPtr)
            If VerificationPattern(pattern) = False Then
                Return Nothing
            End If

            Dim PatternBytes As Byte() = ConvertPatternToBytes(pattern)

            If MyProcess Is Nothing Then
                Console.WriteLine("Processo não encontrado.")
                Return Nothing
            End If

            Const PROCESS_VM_READ As Integer = &H10
            Const PROCESS_QUERY_INFORMATION As Integer = &H400

            Dim byteList As New List(Of Byte)
            Dim wildcards As New List(Of Integer)


            ' Divide a string em pares de dois dígitos
            Dim hexParts As String() = pattern.Split(" "c)


            For i As Integer = 0 To hexParts.Length - 1
                If hexParts(i) = "??" OrElse hexParts(i) = "XX" Then
                    ' Marca a posição do curinga
                    byteList.Add(0) ' Adiciona um valor placeholder, mas não será comparado
                    wildcards.Add(i) ' Adiciona a posição do curinga
                Else
                    ' Converte o par de dígitos hexadecimais em byte
                    byteList.Add(Convert.ToByte(hexParts(i), 16))
                End If
            Next

            Dim PatternResult As Byte() = byteList.ToArray()

            ' Abre o processo com permissões necessárias
            Dim processHandle As IntPtr = OpenProcess(PROCESS_VM_READ Or PROCESS_QUERY_INFORMATION, False, MyProcess.Id)
            If processHandle = IntPtr.Zero Then
                Throw New Exception("Não foi possível abrir o processo.")
            End If

            Dim memoryInfo As New MEMORY_BASIC_INFORMATION()
            Dim baseAddress As IntPtr = IntPtr.Zero
            Dim foundAddresses As New List(Of IntPtr)


            ' Varre a memória do processo
            While VirtualQueryEx(processHandle, baseAddress, memoryInfo, Marshal.SizeOf(memoryInfo)) > 0
                If memoryInfo.State = &H1000 AndAlso (memoryInfo.Protect = &H4 OrElse memoryInfo.Protect = &H20) Then ' Verifica se a memória está acessível
                    Dim buffer(memoryInfo.RegionSize - 1) As Byte
                    Dim bytesRead As Integer

                    ' Lê o bloco de memória
                    If ReadProcessMemory(processHandle, baseAddress, buffer, buffer.Length, bytesRead) Then
                        ' Verifica se o padrão de bytes está presente nesse bloco
                        For i As Integer = 0 To bytesRead - PatternResult.Length
                            Dim found As Boolean = True
                            For j As Integer = 0 To PatternResult.Length - 1
                                ' Ignora as posições com wildcards durante a comparação
                                If Not wildcards.Contains(j) AndAlso buffer(i + j) <> PatternResult(j) Then
                                    found = False
                                    Exit For
                                End If
                            Next

                            If found Then
                                ' Adiciona o endereço encontrado à lista de resultados
                                foundAddresses.Add(baseAddress + i)
                            End If
                        Next
                    End If
                End If
                ' Atualiza o endereço base para a próxima região de memória
                baseAddress = IntPtr.Add(baseAddress.ToInt64(), memoryInfo.RegionSize)
            End While

            Return foundAddresses
        End Function
        Private Function VerificationPattern(ByVal Pattern As String) As Boolean

            Pattern = Pattern.Replace(" ", "").Replace("-", "").Replace(",", "").Replace("?", "0").Replace("x", "0").Replace("X", "0")

            ' Verifica se a string não está vazia e se é composta apenas por caracteres hexadecimais (0-9, A-F)
            If String.IsNullOrEmpty(Pattern) Then
                Return False
            End If

            For Each c As Char In Pattern
                If Not ((c >= "0"c And c <= "9"c) Or (c >= "A"c And c <= "F"c) Or (c >= "a"c And c <= "f"c) Or (c = "?"c) Or (c = "x"c) Or (c = "X"c)) Then
                    Return False
                End If
            Next

            Return True
        End Function
        Private Function VerificationPattern(ByVal Pattern As IntPtr) As Boolean
            ' Verifica se a string não está vazia e se é composta apenas por caracteres hexadecimais (0-9, A-F)
            Dim hex As String = Pattern.ToString("X")

            If String.IsNullOrEmpty(hex) Then
                Return False
            End If

            For Each c As Char In hex

                If Not ((c >= "0"c And c <= "9"c) Or (c >= "A"c And c <= "F"c) Or (c >= "a"c And c <= "f"c)) Then
                    Return False
                End If
            Next

            Return True
        End Function
        Private Function VerificationPattern(ByVal Pattern As Integer) As Boolean
            ' Verifica se a string não está vazia e se é composta apenas por caracteres hexadecimais (0-9, A-F)
            Dim hex As String = Pattern.ToString("X")

            If String.IsNullOrEmpty(hex) Then
                Return False
            End If

            For Each c As Char In hex

                If Not ((c >= "0"c And c <= "9"c) Or (c >= "A"c And c <= "F"c) Or (c >= "a"c And c <= "f"c)) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ' Função para converter string hexadecimal em array de bytes
        Private Function ConvertPatternToBytes(ByVal Pattern As String) As Byte()
            ' Separar a string pelos espaços
            Dim hexValues As String() = Nothing

            If Pattern.Contains("-") Then
                hexValues = Pattern.Split("-"c)
            ElseIf Pattern.Contains(",") Then
                hexValues = Pattern.Split(","c)
            Else
                hexValues = Pattern.Split(" "c)
            End If

            Dim byteList As New List(Of Byte)()

            ' Iterar sobre cada valor hexadecimal
            For Each hexValue As String In hexValues
                If hexValue = "??" Then
                    ' Substituir "??" por FF
                    byteList.Add(&H0)
                ElseIf hexValue.ToUpper = "XX" Then
                    ' Substituir "xx" por FF
                    byteList.Add(&H0)
                ElseIf hexValue = "xx" Then
                    ' Substituir "xx" por FF
                    byteList.Add(&H0)
                Else
                    ' Converter o valor hexadecimal para byte
                    byteList.Add(Convert.ToByte(hexValue, 16))
                End If
            Next

            ' Retornar o array de bytes
            Return byteList.ToArray()
        End Function

        Private Function IsPatternMatch(ByVal memory As Byte(), ByVal position As Integer, ByVal pattern As Byte()) As Boolean
            For i As Integer = 0 To pattern.Length - 1
                If pattern(i) <> memory(position + i) Then
                    Return False
                End If
            Next
            Return True
        End Function

        ' Função personalizada para remover a extensão de um único nome de arquivo
        Private Function RemoveExtension(ByVal fileName As String) As String
            Dim lastDotIndex As Integer = fileName.LastIndexOf(".")
            If lastDotIndex > -1 AndAlso lastDotIndex < fileName.Length - 1 Then
                ' Retorna a parte do nome do arquivo sem a extensão
                Return fileName.Substring(0, lastDotIndex)
            End If
            ' Se não houver extensão, retorna o nome original
            Return fileName
        End Function

        ' Função para converter um array de bytes em IntPtr
        Private Function ByteArrayToIntPtr(ByVal bytes As Byte()) As IntPtr
            ' Cria um ponteiro alocado na memória para o array de bytes
            Dim intPtr As IntPtr = Marshal.AllocHGlobal(bytes.Length)

            ' Copia o conteúdo do array de bytes para a memória apontada por IntPtr
            Marshal.Copy(bytes, 0, intPtr, bytes.Length)

            ' Retorna o IntPtr
            Return intPtr
        End Function
        Private Function ByteArrayToIntPtrArray(ByVal byteArray As Byte()) As IntPtr()
            ' Definir o tamanho de um IntPtr (depende se é 32 ou 64 bits)
            Dim intPtrSize As Integer = IntPtr.Size
            ' Verifica se o comprimento do array de bytes é múltiplo do tamanho de IntPtr
            If byteArray.Length Mod intPtrSize <> 0 Then
                Throw New ArgumentException("O array de bytes não tem um tamanho válido para conversão.")
            End If

            ' Cria um array de IntPtr com o tamanho apropriado
            Dim intPtrArray((byteArray.Length \ intPtrSize) - 1) As IntPtr

            ' Converte cada conjunto de bytes no correspondente IntPtr
            For i As Integer = 0 To intPtrArray.Length - 1
                Dim tempBytes(intPtrSize - 1) As Byte
                Array.Copy(byteArray, i * intPtrSize, tempBytes, 0, intPtrSize)

                ' Converte o array de bytes para IntPtr
                If intPtrSize = 4 Then ' Arquitetura de 32 bits
                    intPtrArray(i) = New IntPtr(BitConverter.ToInt32(tempBytes, 0))
                Else ' Arquitetura de 64 bits
                    intPtrArray(i) = New IntPtr(BitConverter.ToInt64(tempBytes, 0))
                End If
            Next

            Return intPtrArray
        End Function

#End Region




    End Class

End Namespace