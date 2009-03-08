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

Namespace Exceptions

    ''' <summary>
    ''' Invalid Data exception.
    ''' </summary>
    ''' <remarks>
    ''' Raised when a Triple DES encrypt/decrypt operation using variants is performed
    ''' on data that is not 32 or 48 hexadecimal characters long.
    ''' </remarks>
    Public Class XInvalidData
        Inherits Exception

        ''' <summary>
        ''' Exception constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets the exception message.
        ''' </remarks>
        Public Sub New(ByVal description As String)
            MyBase.New(description)
        End Sub
    End Class

End Namespace