Imports System
Imports System.IO

Public Class Form2
    Private Property MoveForm As Boolean
    Private Property MoveForm_MousePosition As Point

    Dim MPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\TRAY\"
    Dim CPath As String = MPath + "Categories\"

    Dim Startup As Boolean = True

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
        My.Settings.DefCategoryT = ComboBox1.SelectedItem.ToString
        My.Settings.DefCategoryN = ComboBox1.SelectedIndex - 1
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
                    ComboBox1.SelectedIndex = My.Settings.DefCategoryN + 1
                Catch ex As Exception
                    My.Settings.DefCategoryN = vbNull
                    My.Settings.DefCategoryT = vbNullString
                End Try
            ComboBox2.SelectedIndex = My.Settings.DefStart
            CheckBox1.Checked = My.Settings.Top
        End If

        If My.Settings.Top = True And Me.TopMost = False Then
            Me.TopMost = True
        ElseIf My.Settings.Top = False And Me.TopMost = True Then
            Me.TopMost = False
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ComboBox1.Items.Clear()
        Startup = True
    End Sub


End Class