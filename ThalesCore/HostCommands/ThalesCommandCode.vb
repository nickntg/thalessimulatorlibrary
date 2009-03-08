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
    ''' The ThalesCommandCode attribute should be attached to all classes that inherit
    ''' from <see cref="HostCommands.AHostCommand"/>.
    ''' </summary>
    ''' <remarks>
    ''' The attribute is parsed at runtime and is used by <see cref="ThalesMain"/>
    ''' to find classes that implement host commands.
    ''' </remarks>
    <AttributeUsage(AttributeTargets.Class)> Public Class ThalesCommandCode
        Inherits Attribute

        ''' <summary>
        ''' Racal Command Code.
        ''' </summary>
        ''' <remarks>
        ''' The command code of the host command implemented by a class.
        ''' </remarks>
        Public CommandCode As String

        ''' <summary>
        ''' Racal Response Code.
        ''' </summary>
        ''' <remarks>
        ''' The response code of the host command implemented by a class.
        ''' </remarks>
        Public ResponseCode As String

        ''' <summary>
        ''' Racal Response Code after I/O.
        ''' </summary>
        ''' <remarks>
        ''' The response code, after I/O is concluded, of the host command implemented by a class.
        ''' </remarks>
        Public ResponseCodeAfterIO As String

        ''' <summary>
        ''' Command description.
        ''' </summary>
        ''' <remarks>
        ''' A description of the host command implemented by a class.
        ''' </remarks>
        Public Description As String

        ''' <summary>
        ''' Command code constructor.
        ''' </summary>
        ''' <remarks>
        ''' This is the constructor of the ThalesCommandCode attribute.
        ''' </remarks>
        Public Sub New(ByVal commandCode As String, ByVal responseCode As String, _
        ByVal responseCodeAfterIO As String, ByVal Description As String)
            Me.CommandCode = commandCode
            Me.ResponseCode = responseCode
            Me.ResponseCodeAfterIO = responseCodeAfterIO
            Me.Description = Description
        End Sub

    End Class

End Namespace