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
    ''' Invalid Account exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an invalid account is encountered.
    ''' </remarks>
    Public Class XInvalidAccount
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

    ''' <summary>
    ''' Invalid PIN length exception.
    ''' </summary>
    ''' <remarks>
    ''' This account is raised if an invalid PIN length is encountered.
    ''' </remarks>
    Public Class XInvalidPINLength
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

    ''' <summary>
    ''' Invalid PIN Block Format exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an invalid PIN Block Format is encountered.
    ''' </remarks>
    Public Class XUnsupportedPINBlockFormat
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

    ''' <summary>
    ''' Invalid Storage File exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an invalid (or empty) storage file is encountered.
    ''' </remarks>
    Public Class XInvalidStorageFile
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

    ''' <summary>
    ''' Encrypt Error exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an error occurs during a DES encrypt operation.
    ''' </remarks>
    Public Class XEncryptError
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

    ''' <summary>
    ''' Decrypt Error exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an error occurs during a DES encrypt operation.
    ''' </remarks>
    Public Class XDecryptError
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

    ''' <summary>
    ''' Invalid Key exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if an invalid key is encountered.
    ''' </remarks>
    Public Class XInvalidKey
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

    ''' <summary>
    ''' Short Message exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised during message parsing if some characters appear to be
    ''' missing from the message.
    ''' </remarks>
    Public Class XShortMessage
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

    ''' <summary>
    ''' No Determiner Matched exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if during parsing of a field that uses a
    ''' <see cref="Message.MessageFieldDeterminerCollection"/>, no determiner
    ''' matches to the message contents.
    ''' </remarks>
    Public Class XNoDeterminerMatched
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

    ''' <summary>
    ''' Invalid Configuration exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is raised if the <see cref="ThalesMain"/> object cannot successfully
    ''' read the XML configuration file.
    ''' </remarks>
    Public Class XInvalidConfiguration
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

    ''' <summary>
    ''' Invalid Key Scheme exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is thrown if an invalid key scheme is passed to a <see cref="KeySchemeTable"/> method.
    ''' </remarks>
    Public Class XInvalidKeyScheme
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

    ''' <summary>
    ''' Invalid Key Type exception.
    ''' </summary>
    ''' <remarks>
    ''' This exception is thrown if an invalid key type is used by a <see cref="KeyTypeTable"/> class method.
    ''' </remarks>
    Public Class XInvalidKeyType
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
