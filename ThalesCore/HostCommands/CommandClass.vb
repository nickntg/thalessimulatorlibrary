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

Namespace HostCommands

    ''' <summary>
    ''' Holds information about loaded command implementations.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class contain information about <see cref="AHostCommand"/> implementations
    ''' of host commands (either buildin or compliled at runtime).
    ''' </remarks>
    Public Class CommandClass
        Implements IComparer(Of CommandClass)

        ''' <summary>
        ''' Command code.
        ''' </summary>
        ''' <remarks>
        ''' The two-character Thales command code.
        ''' </remarks>
        Public CommandCode As String

        ''' <summary>
        ''' Response code.
        ''' </summary>
        ''' <remarks>
        ''' The two-character Thales response code.
        ''' </remarks>
        Public ResponseCode As String

        ''' <summary>
        ''' Response code after I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' The two-character Thales response code after I/O is concluded.
        ''' </remarks>
        Public ResponseCodeAfterIO As String

        ''' <summary>
        ''' The implementation type.
        ''' </summary>
        ''' <remarks>
        ''' A Type with the implementation class type.
        ''' </remarks>
        Public DeclaringType As Type

        ''' <summary>
        ''' Command description.
        ''' </summary>
        ''' <remarks>
        ''' A description of the command's purpose.
        ''' </remarks>
        Public Description As String

        ''' <summary>
        ''' Assembly name.
        ''' </summary>
        ''' <remarks>
        ''' The full name of the assembly containing the implemented class.
        ''' </remarks>
        Public AssemblyName As String

        ''' <summary>
        ''' Class constructor.
        ''' </summary>
        ''' <remarks>
        ''' Class constructor.
        ''' </remarks>
        Public Sub New(ByVal cCode As String, ByVal rCode As String, ByVal rCodeAfterIO As String, _
                       ByVal dclType As Type, ByVal assemblyName As String, _
                       ByVal description As String)
            Me.CommandCode = cCode
            Me.ResponseCode = rCode
            Me.ResponseCodeAfterIO = rCodeAfterIO
            Me.DeclaringType = dclType
            Me.AssemblyName = assemblyName
            Me.Description = description
        End Sub

        ''' <summary>
        ''' Implemented to sort by command code.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Compare(ByVal x As CommandClass, ByVal y As CommandClass) As Integer Implements System.Collections.Generic.IComparer(Of CommandClass).Compare
            Return String.Compare(x.CommandCode, y.CommandCode)
        End Function

    End Class

End Namespace