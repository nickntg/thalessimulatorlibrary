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

Namespace Message.XML

    ''' <summary>
    ''' This class represents a message field, as that is described in an XML definition.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MessageField

        Private m_name As String

        ''' <summary>
        ''' Get/set the field's name.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name() As String
            Get
                Return m_name
            End Get
            Set(ByVal value As String)
                m_name = value
            End Set
        End Property

        Private m_length As Integer

        ''' <summary>
        ''' Get/set the field's length.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Length() As Integer
            Get
                Return m_length
            End Get
            Set(ByVal value As Integer)
                m_length = value
            End Set
        End Property

        Private m_dynamicLength As String

        ''' <summary>
        ''' Get/set the name of the field to lookup
        ''' in order to get the length for this field.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DynamicLength() As String
            Get
                Return m_dynamicLength
            End Get
            Set(ByVal value As String)
                m_dynamicLength = value
            End Set
        End Property

        Private m_parseUntilValue As String = ""

        ''' <summary>
        ''' Get/set the message value until which
        ''' field parsing will continue.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ParseUntilValue() As String
            Get
                Return m_parseUntilValue
            End Get
            Set(ByVal value As String)
                m_parseUntilValue = value
            End Set
        End Property

        Private m_msgFieldType As MessageFieldTypes

        ''' <summary>
        ''' Get/set the field's type.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MessageFieldType() As MessageFieldTypes
            Get
                Return m_msgFieldType
            End Get
            Set(ByVal value As MessageFieldTypes)
                m_msgFieldType = value
            End Set
        End Property

        Private m_dependentField As String

        ''' <summary>
        ''' Get/set the other field upon which this field depends.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DependentField() As String
            Get
                Return m_dependentField
            End Get
            Set(ByVal value As String)
                m_dependentField = value
            End Set
        End Property

        Private m_dependentValue As New List(Of String)

        ''' <summary>
        ''' Get/set the expected value of the other field upon which this field depends.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DependentValue() As List(Of String)
            Get
                Return m_dependentValue
            End Get
            Set(ByVal value As List(Of String))
                m_dependentValue = value
            End Set
        End Property

        Private m_exclusiveDependency As Boolean = True

        ''' <summary>
        ''' Get/set whether the field dependency is exclusive.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExclusiveDependency() As Boolean
            Get
                Return m_exclusiveDependency
            End Get
            Set(ByVal value As Boolean)
                m_exclusiveDependency = value
            End Set
        End Property

        Private m_validValues As New List(Of String)

        ''' <summary>
        ''' Get/set the list of valid values for this field.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ValidValues() As List(Of String)
            Get
                Return m_validValues
            End Get
            Set(ByVal value As List(Of String))
                m_validValues = value
            End Set
        End Property

        Private m_optionValues As New List(Of String)

        ''' <summary>
        ''' Get/set the list of optional values for this field.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OptionValues() As List(Of String)
            Get
                Return m_optionValues
            End Get
            Set(ByVal value As List(Of String))
                m_optionValues = value
            End Set
        End Property

        Private m_rejectionCode As String

        ''' <summary>
        ''' Get/set the Thales rejection code if the field value is invalid.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RejectionCode() As String
            Get
                Return m_rejectionCode
            End Get
            Set(ByVal value As String)
                m_rejectionCode = value
            End Set
        End Property

        Private m_skip As Boolean

        ''' <summary>
        ''' Get/set whether to skip processing for this field.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Skip() As Boolean
            Get
                Return m_skip
            End Get
            Set(ByVal value As Boolean)
                m_skip = value
            End Set
        End Property

        Private m_repetitions As String

        ''' <summary>
        ''' Get/set the number of field repetitions or the name
        ''' of the field with the number of field repetitions.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Repetitions() As String
            Get
                Return m_repetitions
            End Get
            Set(ByVal value As String)
                m_repetitions = value
            End Set
        End Property

        Private m_staticRepetitions As Boolean = False

        ''' <summary>
        ''' Get/set whether this field demands static repetitions.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property StaticRepetitions() As Boolean
            Get
                Return m_staticRepetitions
            End Get
            Set(ByVal value As Boolean)
                m_staticRepetitions = value
            End Set
        End Property

        Private m_skipUntil As Boolean = False

        ''' <summary>
        ''' Get/set whether we'll continue parsing until a valid value is detected.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SkipUntilValid() As Boolean
            Get
                Return m_skipUntil
            End Get
            Set(ByVal value As Boolean)
                m_skipUntil = value
            End Set
        End Property

        ''' <summary>
        ''' Set the dependent value list from a comma-separated string
        ''' of values.
        ''' </summary>
        ''' <param name="s">Comma-separated string of values.</param>
        ''' <remarks></remarks>
        Public Sub SetDependentValue(ByVal s As String)
            Dim sSplit() As String = s.Split(","c)
            m_dependentValue.Clear()
            For Each Str As String In sSplit
                m_dependentValue.Add(Str)
            Next
        End Sub

        ''' <summary>
        ''' Returns a copy of this instance.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone() As MessageField
            Dim o As New MessageField()
            o.DependentField = Me.DependentField
            o.DependentValue = CloneStringList(Me.DependentValue)
            o.ExclusiveDependency = Me.ExclusiveDependency
            o.Length = Me.Length
            o.DynamicLength = Me.DynamicLength
            o.ParseUntilValue = Me.ParseUntilValue
            o.MessageFieldType = Me.MessageFieldType
            o.Name = Me.Name
            o.OptionValues = CloneStringList(Me.OptionValues)
            o.RejectionCode = Me.RejectionCode
            o.Repetitions = Me.Repetitions
            o.Skip = Me.Skip
            o.StaticRepetitions = Me.StaticRepetitions
            o.ValidValues = CloneStringList(Me.ValidValues)
            o.SkipUntilValid = Me.SkipUntilValid
            Return o
        End Function

        Private Function CloneStringList(ByVal lst As List(Of String)) As List(Of String)
            Dim newLst As New List(Of String)
            For Each s As String In lst
                newLst.Add(s)
            Next
            Return newLst
        End Function
    End Class

End Namespace