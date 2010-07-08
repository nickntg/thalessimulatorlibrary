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

Namespace ConsoleCommands

    ''' <summary>
    ''' This class defines an attribute that is used to
    ''' decorate classes that implement console commands.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class)> Public Class ThalesConsoleCommandCode
        Inherits Attribute

        ''' <summary>
        ''' Console Command Code.
        ''' </summary>
        ''' <remarks>
        ''' The console command code of the implementor.
        ''' </remarks>
        Public ConsoleCommandCode As String

        ''' <summary>
        ''' Command description.
        ''' </summary>
        ''' <remarks>
        ''' A description of the console command implemented by a class.
        ''' </remarks>
        Public Description As String

        Public Sub New(ByVal consoleCode As String, ByVal description As String)
            Me.ConsoleCommandCode = consoleCode
            Me.Description = description
        End Sub

    End Class

End Namespace
