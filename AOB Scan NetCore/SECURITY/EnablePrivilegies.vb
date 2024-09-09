Imports System.Runtime.InteropServices

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

Friend Class EnablePrivileges


    <DllImport("advapi32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function LookupPrivilegeValue(ByVal lpSystemName As String,
                                             ByVal lpName As String,
                                             ByRef lpLuid As LUID) As Boolean
    End Function

    ' Use this signature if you do not want the previous state
    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Shared Function AdjustTokenPrivileges(ByVal TokenHandle As IntPtr,
                                              <MarshalAs(UnmanagedType.Bool)> ByVal DisableAllPrivileges As Boolean,
                                              ByRef NewState As TOKEN_PRIVILEGES,
                                              ByVal Zero As UInt32,
                                              ByVal Null1 As IntPtr,
                                              ByVal Null2 As IntPtr) As Boolean
    End Function

    Private Shared STANDARD_RIGHTS_REQUIRED As UInteger = &HF0000
    Private Shared STANDARD_RIGHTS_READ As UInteger = &H20000
    Private Shared TOKEN_ASSIGN_PRIMARY As UInteger = &H1
    Private Shared TOKEN_DUPLICATE As UInteger = &H2
    Private Shared TOKEN_IMPERSONATE As UInteger = &H4
    Private Shared TOKEN_QUERY As UInteger = &H8
    Private Shared TOKEN_QUERY_SOURCE As UInteger = &H10
    Private Shared TOKEN_ADJUST_PRIVILEGES As UInteger = &H20
    Private Shared TOKEN_ADJUST_GROUPS As UInteger = &H40
    Private Shared TOKEN_ADJUST_DEFAULT As UInteger = &H80
    Private Shared TOKEN_ADJUST_SESSIONID As UInteger = &H100
    Private Shared TOKEN_READ As UInteger = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
    Private Shared TOKEN_ALL_ACCESS As UInteger = (STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or
                                                TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or
                                                TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or
                                                TOKEN_ADJUST_SESSIONID)

    Private Const SE_ASSIGNPRIMARYTOKEN_NAME As String = "SeAssignPrimaryTokenPrivilege"
    Private Const SE_AUDIT_NAME As String = "SeAuditPrivilege"
    Private Const SE_BACKUP_NAME As String = "SeBackupPrivilege"
    Private Const SE_CHANGE_NOTIFY_NAME As String = "SeChangeNotifyPrivilege"
    Private Const SE_CREATE_GLOBAL_NAME As String = "SeCreateGlobalPrivilege"
    Private Const SE_CREATE_PAGEFILE_NAME As String = "SeCreatePagefilePrivilege"
    Private Const SE_CREATE_PERMANENT_NAME As String = "SeCreatePermanentPrivilege"
    Private Const SE_CREATE_SYMBOLIC_LINK_NAME As String = "SeCreateSymbolicLinkPrivilege"
    Private Const SE_CREATE_TOKEN_NAME As String = "SeCreateTokenPrivilege"
    Private Const SE_DEBUG_NAME As String = "SeDebugPrivilege"
    Private Const SE_ENABLE_DELEGATION_NAME As String = "SeEnableDelegationPrivilege"
    Private Const SE_IMPERSONATE_NAME As String = "SeImpersonatePrivilege"
    Private Const SE_INC_BASE_PRIORITY_NAME As String = "SeIncreaseBasePriorityPrivilege"
    Private Const SE_INCREASE_QUOTA_NAME As String = "SeIncreaseQuotaPrivilege"
    Private Const SE_INC_WORKING_SET_NAME As String = "SeIncreaseWorkingSetPrivilege"
    Private Const SE_LOAD_DRIVER_NAME As String = "SeLoadDriverPrivilege"
    Private Const SE_LOCK_MEMORY_NAME As String = "SeLockMemoryPrivilege"
    Private Const SE_MACHINE_ACCOUNT_NAME As String = "SeMachineAccountPrivilege"
    Private Const SE_MANAGE_VOLUME_NAME As String = "SeManageVolumePrivilege"
    Private Const SE_PROF_SINGLE_PROCESS_NAME As String = "SeProfileSingleProcessPrivilege"
    Private Const SE_RELABEL_NAME As String = "SeRelabelPrivilege"
    Private Const SE_REMOTE_SHUTDOWN_NAME As String = "SeRemoteShutdownPrivilege"
    Private Const SE_RESTORE_NAME As String = "SeRestorePrivilege"
    Private Const SE_SECURITY_NAME As String = "SeSecurityPrivilege"
    Private Const SE_SHUTDOWN_NAME As String = "SeShutdownPrivilege"
    Private Const SE_SYNC_AGENT_NAME As String = "SeSyncAgentPrivilege"
    Private Const SE_SYSTEM_ENVIRONMENT_NAME As String = "SeSystemEnvironmentPrivilege"
    Private Const SE_SYSTEM_PROFILE_NAME As String = "SeSystemProfilePrivilege"
    Private Const SE_SYSTEMTIME_NAME As String = "SeSystemtimePrivilege"
    Private Const SE_TAKE_OWNERSHIP_NAME As String = "SeTakeOwnershipPrivilege"
    Private Const SE_TCB_NAME As String = "SeTcbPrivilege"
    Private Const SE_TIME_ZONE_NAME As String = "SeTimeZonePrivilege"
    Private Const SE_TRUSTED_CREDMAN_ACCESS_NAME As String = "SeTrustedCredManAccessPrivilege"
    Private Const SE_UNDOCK_NAME As String = "SeUndockPrivilege"
    Private Const SE_UNSOLICITED_INPUT_NAME As String = "SeUnsolicitedInputPrivilege"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure LUID
        Public LowPart As UInt32
        Public HighPart As Int32
    End Structure


    Protected Enum AllocationProtectEnum As UInteger
        PAGE_EXECUTE = &H10
        PAGE_EXECUTE_READ = &H20
        PAGE_EXECUTE_READWRITE = &H40
        PAGE_EXECUTE_WRITECOPY = &H80
        PAGE_NOACCESS = &H1
        PAGE_READONLY = &H2
        PAGE_READWRITE = &H4
        PAGE_WRITECOPY = &H8
        PAGE_GUARD = &H100
        PAGE_NOCACHE = &H200
        PAGE_WRITECOMBINE = &H400
    End Enum

    Protected Enum StateEnum As UInteger
        MEM_COMMIT = &H1000
        MEM_FREE = &H10000
        MEM_RESERVE = &H2000
    End Enum

    Private Enum TypeEnum As UInteger
        MEM_IMAGE = &H1000000
        MEM_MAPPED = &H40000
        MEM_PRIVATE = &H20000
    End Enum

    Public Const SE_PRIVILEGE_ENABLED_BY_DEFAULT As UInt32 = &H1
    Public Const SE_PRIVILEGE_ENABLED As UInt32 = &H2
    Public Const SE_PRIVILEGE_REMOVED As UInt32 = &H4
    Public Const SE_PRIVILEGE_USED_FOR_ACCESS As UInt32 = &H8000000

    <StructLayout(LayoutKind.Sequential)>
    Private Structure TOKEN_PRIVILEGES
        Public PrivilegeCount As UInt32
        Public Luid As LUID
        Public Attributes As UInt32
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure LUID_AND_ATTRIBUTES
        Public Luid As LUID
        Public Attributes As UInt32
    End Structure
    Friend Shared Sub GoDebugPriv()
        Dim Memory_ As New Memory
        Dim hToken As IntPtr
        Dim luidSEDebugNameValue As LUID
        Dim tkpPrivileges As TOKEN_PRIVILEGES

        If Not Memory.OpenProcessToken(Memory.GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES Or TOKEN_QUERY, hToken) Then
            Return
        End If

        If Not LookupPrivilegeValue(Nothing, SE_DEBUG_NAME, luidSEDebugNameValue) Then
            Memory.CloseHandle(hToken)
            Return
        End If

        tkpPrivileges.PrivilegeCount = 1
        tkpPrivileges.Luid = luidSEDebugNameValue
        tkpPrivileges.Attributes = SE_PRIVILEGE_ENABLED
        AdjustTokenPrivileges(hToken, False, tkpPrivileges, 0, IntPtr.Zero, IntPtr.Zero)
        Memory.CloseHandle(hToken)


    End Sub
End Class

