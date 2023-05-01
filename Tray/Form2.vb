Imports System
Imports System.IO

Public Class Form2
    Private Property MoveForm As Boolean
    Private Property MoveForm_MousePosition As Point

    Dim MPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\TRAY\"
    Dim CPath As String = MPath + "Categories\"

    Dim Startup As Boolean = True
    Dim ctrl As Boolean = False

    '------------------------------------------

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        'Çerçeveli uygulama - 1
        If e.Button = MouseButtons.Left Then
            MoveForm = True
            Me.Cursor = Cursors.Default
            MoveForm_MousePosition = e.Location
        End If
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        'Çerçeveli uygulama - 2
        If MoveForm Then
            Me.Location = Me.Location + (e.Location - MoveForm_MousePosition)
        End If
    End Sub

    Private Sub Panel1_MouseUp(sender As Object, e As MouseEventArgs) Handles Panel1.MouseUp
        'Çerçeveli uygulama - 3
        If e.Button = MouseButtons.Left Then
            MoveForm = False
            Me.Cursor = Cursors.Default
        End If
    End Sub

    '-------------------------------------
    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Form1.Show()
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Başlangıç
        Timer1.Enabled = True
    End Sub

'-------------------------------------

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        'Kapatma butonu
        Me.Close()
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        'Küçültme butonu
        Me.WindowState = FormWindowState.Minimized
    End Sub

'-------------------------------------

'-------------------------------------

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            If ctrl = True Then
                If Not Directory.Exists(CPath + ComboBox1.SelectedItem.ToString) And Not ComboBox1.SelectedItem.ToString = "<NONE>" Then
                    ComboBox1.Items.Clear()
                    Startup = True
                Else
                    My.Settings.DefCategoryT = ComboBox1.SelectedItem.ToString
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        My.Settings.DefStart = ComboBox2.SelectedIndex
    End Sub

'-------------------------------------

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        My.Settings.Top = CheckBox1.Checked
    End Sub

    '-------------------------------------

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Kategorileri getir ve bazı kontrolleri yap
        Dim control As Boolean = False

        Try
            Dim Kisimleri = My.Computer.FileSystem.GetDirectories(CPath)
            Dim Ksayisi = My.Computer.FileSystem.GetDirectories(CPath).Count
            If Not ComboBox1.Items.Count = Ksayisi + 1 Then
                ComboBox1.Items.Clear()
                ComboBox1.Items.Add("<NONE>")
                For Each isim As String In Kisimleri
                    Dim result As String = Path.GetFileName(isim)
                    ComboBox1.Items.Add(result)
                Next
            End If
        Catch ex As Exception
        End Try

        If Startup = True Then
            Startup = False

            Try
                For i As Integer = 0 To ComboBox1.Items.Count
                    Dim name As String = ComboBox1.GetItemText(ComboBox1.Items(i))
                    If name = My.Settings.DefCategoryT Then
                        ComboBox1.SelectedIndex = i
                        control = True
                    ElseIf Not control = True Then
                        ComboBox1.SelectedIndex = 0
                    End If
                Next
            Catch ex As Exception
            End Try
            'My.Settings.DefCategoryT = vbNullString

            ComboBox2.SelectedIndex = My.Settings.DefStart
            CheckBox1.Checked = My.Settings.Top
        End If

        If My.Settings.Top = True And Me.TopMost = False Then
            Me.TopMost = True
        ElseIf My.Settings.Top = False And Me.TopMost = True Then
            Me.TopMost = False
        End If
        ctrl = True

    End Sub

End Class