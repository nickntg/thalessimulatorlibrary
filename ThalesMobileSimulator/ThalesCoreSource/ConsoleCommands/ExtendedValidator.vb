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
    ''' This class allows console commands to chain validators in order to
    ''' perform several checks in sequence.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExtendedValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Internal storage for the next validator in the chain.
        ''' </summary>
        ''' <remarks></remarks>
        Protected m_nextVal As ExtendedValidator = Nothing

        ''' <summary>
        ''' Current validator to be used.
        ''' </summary>
        ''' <remarks></remarks>
        Protected m_currentVal As IConsoleDataValidator

        ''' <summary>
        ''' Creates an extended validator instance.
        ''' </summary>
        ''' <param name="validator">Validator instance.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal validator As IConsoleDataValidator)
            m_currentVal = validator
        End Sub

        ''' <summary>
        ''' Adds another validator to the chain.
        ''' </summary>
        ''' <param name="nextValidator">Next validator instance.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Note that any validator added with this method cannot be the <b>last</b> validator.
        ''' To add the last validator see the <see cref="AddLast" /> method of this class.
        ''' </remarks>
        Public Function Add(ByVal nextValidator As IConsoleDataValidator) As ExtendedValidator
            m_nextVal = New ExtendedValidator(nextValidator)
            Return m_nextVal
        End Function

        ''' <summary>
        ''' Adds the last validator to the chain.
        ''' </summary>
        ''' <param name="nextValidator">Last validator instance.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Note that the validator added with this method must be the <b>last</b> validator.
        ''' To add any validator other than the last see the <see cref="Add" /> method of this class.
        ''' </remarks>
        Public Function AddLast(ByVal nextValidator As IConsoleDataValidator) As ExtendedValidator
            m_nextVal = New ExtendedValidator(nextValidator)
            Return Me
        End Function

        ''' <summary>
        ''' Runs through all validators in the chain.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks></remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            m_currentVal.ValidateConsoleMessage(consoleMsg)
            If m_nextVal IsNot Nothing Then
                m_nextVal.ValidateConsoleMessage(consoleMsg)
            End If
        End Sub
    End Class

End Namespace