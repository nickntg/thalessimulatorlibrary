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
''' Class to abstract the Racal key scheme table.
''' </summary>
''' <remarks>Class to abstract the Racal key scheme table.</remarks>
Public Class KeySchemeTable

    ''' <summary>
    ''' Enumerates the Racal key scheme table values.
    ''' </summary>
    ''' <remarks>
    ''' Enumerates the Racal key scheme table values.
    ''' </remarks>
    Public Enum KeyScheme
        ''' <summary>
        ''' Single-length DES key encrypted using ANSI X9.17.
        ''' </summary>
        ''' <remarks>
        ''' Single-length DES key encrypted using ANSI X9.17.
        ''' </remarks>
        SingleDESKey = 0

        ''' <summary>
        ''' Double-length key encrypted using the Variant method.
        ''' </summary>
        ''' <remarks>
        ''' Double-length key encrypted using the Variant method.
        ''' </remarks>
        DoubleLengthKeyVariant = 1

        ''' <summary>
        ''' Triple-length key encrypted using the Variant method.
        ''' </summary>
        ''' <remarks>
        ''' Triple-length key encrypted using the Variant method.
        ''' </remarks>
        TripleLengthKeyVariant = 2

        ''' <summary>
        ''' Double-length key encrypted using the ANSI X9.17 method.
        ''' </summary>
        ''' <remarks></remarks>
        DoubleLengthKeyAnsi = 3

        ''' <summary>
        ''' Triple-length key encrypted using the ANSI X9.17 method.
        ''' </summary>
        ''' <remarks>
        ''' Triple-length key encrypted using the ANSI X9.17 method.
        ''' </remarks>
        TripleLengthKeyAnsi = 4

        ''' <summary>
        ''' Unspecified key scheme code.
        ''' </summary>
        ''' <remarks>
        ''' Unspecified key scheme code.
        ''' </remarks>
        Unspecified = 5
    End Enum

    ''' <summary>
    ''' Returns a key scheme character.
    ''' </summary>
    ''' <remarks>
    ''' This method returns one of Z, X, Y, U or T characters, depending upon the parameter.
    ''' </remarks>
    Public Shared Function GetKeySchemeValue(ByVal key As KeyScheme) As String
        Select Case key
            Case KeyScheme.DoubleLengthKeyAnsi
                Return "X"
            Case KeyScheme.DoubleLengthKeyVariant
                Return "U"
            Case KeyScheme.SingleDESKey
                Return "Z"
            Case KeyScheme.TripleLengthKeyAnsi
                Return "Y"
            Case KeyScheme.TripleLengthKeyVariant
                Return "T"
            Case Else
                Throw New Exceptions.XInvalidKeyScheme("Invalid key scheme")
        End Select
    End Function

    ''' <summary>
    ''' Returns a key scheme type.
    ''' </summary>
    ''' <remarks>
    ''' The method parses the input parameter to a key scheme value.
    ''' </remarks>
    Public Shared Function GetKeySchemeFromValue(ByVal v As String) As KeyScheme
        If v Is Nothing OrElse v.Length <> 1 Then Throw New Exceptions.XInvalidKeyScheme("Invalid key scheme")

        Select Case v
            Case "X"
                Return KeyScheme.DoubleLengthKeyAnsi
            Case "U"
                Return KeyScheme.DoubleLengthKeyVariant
            Case "Z"
                Return KeyScheme.SingleDESKey
            Case "T"
                Return KeyScheme.TripleLengthKeyVariant
            Case "Y"
                Return KeyScheme.TripleLengthKeyAnsi
            Case "0"
                Return KeyScheme.Unspecified
            Case Else
                Throw New Exceptions.XInvalidKeyScheme("Invalid key scheme " + v)
        End Select

    End Function

End Class
