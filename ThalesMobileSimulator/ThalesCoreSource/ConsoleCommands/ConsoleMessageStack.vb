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
    ''' This class holds a stack of console messages.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConsoleMessageStack

        ''' <summary>
        ''' Internal stack storage.
        ''' </summary>
        ''' <remarks></remarks>
        Protected m_stack As Stack(Of ConsoleMessage) = New Stack(Of ConsoleMessage)

        ''' <summary>
        ''' Pushes a message to the stack.
        ''' </summary>
        ''' <param name="msg">Console message instance.</param>
        ''' <remarks></remarks>
        Public Sub PushToStack(ByVal msg As ConsoleMessage)
            m_stack.Push(msg)
        End Sub

        ''' <summary>
        ''' Pops and returns a message from the stack.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PopFromStack() As ConsoleMessage
            Return m_stack.Pop
        End Function

        ''' <summary>
        ''' Returns the number of messages on the stack.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MessagesOnStack() As Integer
            Return m_stack.Count
        End Function

    End Class

End Namespace