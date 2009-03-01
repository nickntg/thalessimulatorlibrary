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

Namespace Cryptography.LMK

    ''' <summary>
    ''' Racal variants abstraction.
    ''' </summary>
    ''' <remarks>
    ''' This class provides an abstraction for retrieving the variant values used 
    ''' with encryption/decryption operations with the variant method.
    ''' </remarks>
    Public Class Variants

        Private Shared _Variants() As String = {"A6", "5A", "6A", "DE", "2B", "50", "74", "9C", "FA"}
        Private Shared _doubleKeyVariants() As String = {"A6", "5A"}
        Private Shared _tripleKeyVariants() As String = {"6A", "DE", "2B"}

        ''' <summary>
        ''' Returns a variant value.
        ''' </summary>
        ''' <remarks>
        ''' Returns a variant Racal value.
        ''' </remarks>
        Public Shared ReadOnly Property VariantNbr(ByVal index As Integer) As String
            Get
                Return _Variants(index - 1)
            End Get
        End Property

        ''' <summary>
        ''' Returns a variant value.
        ''' </summary>
        ''' <remarks>
        ''' Returns a variant Racal value used for double-length encryption/decryption.
        ''' </remarks>
        Public Shared ReadOnly Property DoubleLengthVariant(ByVal index As Integer) As String
            Get
                Return _doubleKeyVariants(index - 1)
            End Get
        End Property

        ''' <summary>
        ''' Returns a variant value.
        ''' </summary>
        ''' <remarks>
        ''' Returns a variant Racal value used for triple-length encryption/decryption.
        ''' </remarks>
        Public Shared ReadOnly Property TripleLengthVariant(ByVal index As Integer) As String
            Get
                Return _tripleKeyVariants(index - 1)
            End Get
        End Property

    End Class

End Namespace
