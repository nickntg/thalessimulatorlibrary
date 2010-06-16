''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 

''' <summary>
''' Provides in-memory storage.
''' </summary>
''' <remarks>
''' This class provides memory storage for application-wide variables.
''' </remarks>
Public Class Resources

    ''' <summary>
    ''' Resource name for HSM firmware number.
    ''' </summary>
    ''' <remarks>
    ''' Resource for HSM firmware number.
    ''' </remarks>
    Public Const FIRMWARE_NUMBER As String = "FIRMWARE_NUMBER"

    ''' <summary>
    ''' Resource name for HSM DSP firmware number.
    ''' </summary>
    ''' <remarks>
    ''' Resource name for HSM DSP firmware number.
    ''' </remarks>
    Public Const DSP_FIRMWARE_NUMBER As String = "DSP_FIRMWARE_NUMBER"

    ''' <summary>
    ''' Resource name for HSM maximum TCP connections.
    ''' </summary>
    ''' <remarks>
    ''' Resource name for HSM maximum TCP connections.
    ''' </remarks>
    Public Const MAX_CONS As String = "MAX_CONS"

    ''' <summary>
    ''' Resource name for boolean flag indicating whether the HSM is in the authorized state.
    ''' </summary>
    ''' <remarks>
    ''' Resource name for boolean flag indicating whether the HSM is in the authorized state.
    ''' </remarks>
    Public Const AUTHORIZED_STATE As String = "AUTH_STATE"

    ''' <summary>
    ''' Resource name for the HSM LMK check value.
    ''' </summary>
    ''' <remarks>
    ''' Resource name for the HSM LMK check value.
    ''' </remarks>
    Public Const LMK_CHECK_VALUE As String = "LMK_CHECK_VALUE"

    ''' <summary>
    ''' Resource name for the clear PIN length.
    ''' </summary>
    ''' <remarks>
    ''' Resource name for the clear PIN length.
    ''' </remarks>
    Public Const CLEAR_PIN_LENGTH As String = "CLEAR_PIN_LENGTH"

    ''' <summary>
    ''' Resource name for the host port.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const WELL_KNOWN_PORT As String = "WELL_KNOWN_PORT"

    ''' <summary>
    ''' Resource name for the console port.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const CONSOLE_PORT As String = "CONSOLE_PORT"

    ''' <summary>
    ''' Resource name for host commands XML definitions.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const HOST_COMMANDS_XML_DEFS As String = "HOST_COMMANDS_XML_DEFS"

    ''' <summary>
    ''' Resource name for double length ZMKs flag.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const DOUBLE_LENGTH_ZMKS As String = "DOUBLE_LENGTH_ZMKS"

    Private Shared _lst As New SortedList(Of String, Object)

    ''' <summary>
    ''' Cleans up all resources.
    ''' </summary>
    ''' <remarks>
    ''' This method cleans up all resources that are present in the hash table.
    ''' </remarks>
    Public Shared Sub CleanUp()
        _lst.Clear()
    End Sub

    ''' <summary>
    ''' Adds a resource to memory.
    ''' </summary>
    ''' <remarks>
    ''' This method adds a resource to the memory store.
    ''' </remarks>
    Public Shared Sub AddResource(ByVal key As String, ByVal value As Object)
        _lst.Add(key, value)
    End Sub

    ''' <summary>
    ''' Returns a resource value.
    ''' </summary>
    ''' <remarks>
    ''' This method returns a resource from the memory store. If the resource name does
    ''' not exist, Nothing is returned.
    ''' </remarks>
    Public Shared Function GetResource(ByVal key As String) As Object
        Return _lst(key)
    End Function

    ''' <summary>
    ''' Updates a resource.
    ''' </summary>
    ''' <remarks>
    ''' Updates a resource value. If the resource does not exist, it is added.
    ''' </remarks>
    Public Shared Sub UpdateResource(ByVal key As String, ByVal value As Object)
        _lst.Remove(key)
        _lst.Add(key, value)
    End Sub

End Class
