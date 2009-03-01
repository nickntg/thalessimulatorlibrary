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

        Private _commandTypes As New Hashtable

        ''' <summary>
        ''' CommandExplorer constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor will search the loaded assemblies for classes that have the
        ''' <see cref="HostCommands.ThalesCommandCode"/> attribute.
        ''' </remarks>
        Public Sub New()

            Dim asm() As [Assembly] = System.AppDomain.CurrentDomain.GetAssemblies()
            For i As Integer = 0 To asm.GetUpperBound(0)
                Dim t As Type
                For Each t In asm(i).GetTypes()
                    Dim atr As Attribute
                    For Each atr In t.GetCustomAttributes(False)
                        If atr.GetType() Is GetType(ThalesSim.Core.HostCommands.ThalesCommandCode) Then
                            If _commandTypes.Item(CType(atr, ThalesCommandCode).CommandCode) Is Nothing Then
                                _commandTypes.Add(CType(atr, ThalesCommandCode).CommandCode, _
                                                  New CommandClass(CType(atr, ThalesCommandCode).CommandCode, _
                                                                   CType(atr, ThalesCommandCode).ResponseCode, _
                                                                   CType(atr, ThalesCommandCode).ResponseCodeAfterIO, _
                                                                   t, asm(i).FullName, _
                                                                   CType(atr, ThalesCommandCode).Description))
                            End If
                        End If
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
            Dim x As IDictionaryEnumerator = _commandTypes.GetEnumerator
            x.Reset()
            While x.MoveNext
                Dim o As CommandClass = CType(x.Value, CommandClass)
                s = s + "Command code: " + o.CommandCode + vbCrLf + _
                        "Response code: " + o.ResponseCode + vbCrLf + _
                        "Type: " + o.DeclaringType.FullName() + vbCrLf + _
                        "Description: " + o.Description + vbCrLf + vbCrLf
            End While
            x = Nothing
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
            Return CType(_commandTypes(commandCode), CommandClass)
        End Function

    End Class

    ''' <summary>
    ''' Holds information about loaded command implementations.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class contain information about <see cref="AHostCommand"/> implementations
    ''' of host commands (either buildin or compliled at runtime).
    ''' </remarks>
    Public Class CommandClass

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
    End Class

End Namespace
