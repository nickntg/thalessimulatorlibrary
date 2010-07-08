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

Imports System.Reflection

Namespace HostCommands

    ''' <summary>
    ''' Used to find class that implement host commands.
    ''' </summary>
    ''' <remarks>
    ''' The command explorer is used to find classes amongst the loaded assemblies that
    ''' have the <see cref="HostCommands.ThalesCommandCode"/> attribute.
    ''' </remarks>
    Public Class CommandExplorer

        Private _commandTypes As New SortedList(Of String, CommandClass)

        ''' <summary>
        ''' CommandExplorer constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor will search the loaded assemblies for classes that have the
        ''' <see cref="HostCommands.ThalesCommandCode"/> attribute.
        ''' </remarks>
        Public Sub New()

            ''We assume a single assembly for the mobile version.
            Dim asm() As [Assembly] = New [Assembly]() {System.Reflection.Assembly.GetExecutingAssembly()}
            For i As Integer = 0 To asm.GetUpperBound(0)
                Dim t As Type
                For Each t In asm(i).GetTypes()
                    Dim atr As Attribute
                    For Each atr In t.GetCustomAttributes(False)
                        If atr.GetType() Is GetType(ThalesSim.Core.HostCommands.ThalesCommandCode) Then
                            Try
                                _commandTypes.Add(CType(atr, ThalesCommandCode).CommandCode, _
                                                  New CommandClass(CType(atr, ThalesCommandCode).CommandCode, _
                                                                   CType(atr, ThalesCommandCode).ResponseCode, _
                                                                   CType(atr, ThalesCommandCode).ResponseCodeAfterIO, _
                                                                   t, asm(i).FullName, _
                                                                   CType(atr, ThalesCommandCode).Description))
                            Catch ex As ArgumentException
                                'We ignore attempts to add duplicates - this may happen when running the unit tests.
                            End Try
                        End If
                        'End If
                    Next
                Next
            Next

        End Sub

        ''' <summary>
        ''' Returns a summary of all loaded host commands.
        ''' </summary>
        ''' <remarks>
        ''' The method returns a summary description of all loaded host commands (classes that
        ''' inherit from <see cref="HostCommands.AHostCommand"/> and declare the 
        ''' <see cref="HostCommands.ThalesCommandCode"/> attribute).
        ''' </remarks>
        Public Function GetLoadedCommands() As String
            Dim s As String = ""
            Dim en As IEnumerator(Of KeyValuePair(Of String, CommandClass)) = _commandTypes.GetEnumerator()

            'Removed unnecessary Reset() for Mono compatibility.
            'en.Reset()

            While en.MoveNext
                s = s + "Command code: " + en.Current.Value.CommandCode + vbCrLf + _
                        "Response code: " + en.Current.Value.ResponseCode + vbCrLf + _
                        "Type: " + en.Current.Value.DeclaringType.FullName() + vbCrLf + _
                        "Description: " + en.Current.Value.Description + vbCrLf + vbCrLf
            End While
            en.Dispose()
            en = Nothing
            Return s
        End Function

        ''' <summary>
        ''' Returns a <see cref="CommandClass"/> object for a specified command.
        ''' </summary>
        ''' <remarks>
        ''' Returns a <see cref="CommandClass"/> object for a specified command. If the
        ''' command is not implemented, Nothing is returned.
        ''' </remarks>
        Public Function GetLoadedCommand(ByVal commandCode As String) As CommandClass
            Try
                Return _commandTypes(commandCode)
            Catch ex As KeyNotFoundException
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Clears the sorted list with the loaded commands.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearLoadedCommands()
            _commandTypes.Clear()
        End Sub

    End Class

End Namespace
