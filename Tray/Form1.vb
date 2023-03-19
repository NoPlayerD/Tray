Imports System
Imports System.IO

Public Class Form1

    Private Property MoveForm As Boolean
    Private Property MoveForm_MousePosition As Point

    Dim MPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\TRAY\"
    Dim CPath As String = MPath + "Categories\"

    Dim CategorySlide As Boolean = False
    Dim CategoryMode As Integer
    Dim OldMode As Integer

    Dim DelStat As String = "disabled"
    Dim DelMode As Integer = 0S

    Dim Startup As Boolean = True
    
    '---------------------------------------------

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Çalışınca:
        DataBaseCheck()
        
        Label1.Text = Application.ProductName + " - " + Application.ProductVersion

        If My.Settings.CategoryMode = 0 Then
            My.Settings.CategoryMode = 1
        End If
        
        CategoryMode = My.Settings.CategoryMode

        ToolStripMenuItem4.Text = CategoryMode.ToString
        OldMode = 0

        Timer2.Start()
        Timer4.Start()
        Timer5.Start()

        Stretch()

    End Sub
    '---------------------------------------------
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
    '---------------------------------------------
    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        'Kapat butonu
        Application.Exit()
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        'TamEkran butonu
        FullScreen()
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        'Küçültme butonu
        Me.WindowState = FormWindowState.Minimized
    End Sub

'------------------------------------------

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Kategori timer'i
        If CategorySlide = True And Not Panel2.Width = 200 Then
            Panel2.Width = Panel2.Width + 30
            ListBox1.Width = ListBox1.Width + 30
        ElseIf CategorySlide = False And Not Panel2.Width = 50 Then
            ListBox1.Width = ListBox1.Width - 30
            Panel2.Width = Panel2.Width - 30
        End If

        If CategorySlide = True And Panel2.Width = 200 Then
            CategorySlide = False
            Timer1.Enabled = False
        ElseIf CategorySlide = False And Panel2.Width = 50 Then
            Timer1.Enabled = False
        End If

    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        'Fare kategori sekmesinde değilse sekmeyi kapat vs.
        If Not Panel2.ClientRectangle.Contains(Panel2.PointToClient(Cursor.Position)) = True Then
            OpenCategory(False)
        End If

        If ToolStripMenuItem4.Text = 2 Then
            Panel2.Width = 200
            ListBox1.Width = 190
        ElseIf ToolStripMenuItem4.Text = 3 Then
            Panel2.Width = 50
            ListBox1.Width = 40
        End If

        If Not OldMode = ToolStripMenuItem4.Text Then
            OldMode = ToolStripMenuItem4.Text
            Stretch()
        End If

        If Not My.Settings.CategoryMode = CategoryMode Then
            My.Settings.CategoryMode = CategoryMode
        End If

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        'kategori reload (listbox1)
        Try
            Dim Kisimleri = My.Computer.FileSystem.GetDirectories(CPath)
            Dim Ksayisi = My.Computer.FileSystem.GetDirectories(CPath).Count
            If Not ListBox1.Items.Count = Ksayisi Then
                ListBox1.Items.Clear()
                For Each isim As String In Kisimleri
                    Dim result As String = Path.GetFileName(isim)
                    ListBox1.Items.Add(result)
                Next
            End If
        Catch ex As Exception
        End Try

        Try
            If Startup = True Then
                Startup = False
                If ListBox1.GetItemText(ListBox1.Items(My.Settings.DefCategoryN)) = My.Settings.DefCategoryT Then
                    ListBox1.SelectedIndex = My.Settings.DefCategoryN
                End If
                If My.Settings.DefStart = 1 Then
                    FullScreen()
                End If
            End If
        Catch ex As Exception
            Startup = False
        End Try
    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        'Reloads
        Try
            If Not ListBox1.SelectedIndex < 0 Then
                Dim FileCount As Integer = My.Computer.FileSystem.GetFiles(CPath + ListBox1.SelectedItem.ToString).Count
                Dim FolderCount As Integer = My.Computer.FileSystem.GetDirectories(CPath + ListBox1.SelectedItem.ToString).Count

                Dim List1Count As Integer
                Dim List2Count As Integer

                If ListView1.Items.Count > 0 Then List1Count = ListView1.Items.Count
                If ListView2.Items.Count > 0 Then List2Count = ListView2.Items.Count

                If Not List1Count = FileCount Then
                    Relaods("file")
                End If
                If Not List2Count = FolderCount Then
                    Relaods("folder")
                End If
            End If

        Catch ex As Exception
            ListBox1.Items.Clear()
            ListView1.Items.Clear()
            imageList1.Images.Clear()

            ListView2.Items.Clear()
        End Try

        If My.Settings.Top = True And Me.TopMost = False Then
            Me.TopMost = True
        ElseIf My.Settings.Top = False And Me.TopMost = True Then
            Me.TopMost = False
        End If

    End Sub

    Private Sub Timer5_Tick(sender As Object, e As EventArgs) Handles Timer5.Tick
        'Seçili kategori yok ise
        Dim bool As Boolean
        If Not ListBox1.SelectedIndex > -1 Then
            bool = False
        Else
            bool = True
        End If

        Button2.Enabled = bool
        AddFileToolStripMenuItem.Enabled = bool
        AddFolderToolStripMenuItem.Enabled = bool
        CreateFolderToolStripMenuItem.Enabled = bool

        FolderToolStripMenuItem.Enabled = bool
        FileToolStripMenuItem.Enabled = bool

        If DelStat = "enabled" Then
            GroupBox1.Visible = True
            DelStat = "active"
        ElseIf DelStat = "disabled" Then
            GroupBox1.Visible = False
        End If

    End Sub

'------------------------------------------

    Private Function Relaods(type As String)
        'Reloads of listview 1 and 2
        Try
            If type = "file" Then
                imageList1.Images.Clear()
                ListView1.Items.Clear()
                ListView1.BeginUpdate()

                Dim di As New IO.DirectoryInfo(CPath + ListBox1.SelectedItem.ToString)
                For Each fi As IO.FileInfo In di.GetFiles("*")
                    Dim icons As Icon = SystemIcons.WinLogo
                    Dim li As New ListViewItem(fi.Name, 1)
                    If Not (imageList1.Images.ContainsKey(fi.Name)) Then
                        icons = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)
                        imageList1.Images.Add(fi.Name, icons)
                    End If
                    icons = Icon.ExtractAssociatedIcon(fi.FullName)
                    imageList1.Images.Add(icons)
                    ListView1.Items.Add(fi.Name, fi.Name)
                Next
                ListView1.EndUpdate()
            End If
            If type = "folder" Then
                Dim dsayisi As Integer = My.Computer.FileSystem.GetDirectories(CPath + ListBox1.SelectedItem.ToString).Count
                Dim odsayisi As Integer = ListView2.Items.Count
                If Not odsayisi = dsayisi Then
                    ListView2.Items.Clear()
                    For Each kats In My.Computer.FileSystem.GetDirectories(CPath + ListBox1.SelectedItem.ToString)
                        Dim sonuc As String = kats.Split("\").Last
                        ListView2.Items.Add(sonuc, 0)
                    Next
                End If
            End If
        Catch ex As Exception
            ListBox1.Items.Clear()
        End Try

        Return True
    End Function

    Private Function Stretch()
        'Listview'leri panel'e ve ekrana göre ayalama
        Dim freespace As Integer = (Me.Width - Panel2.Width - 30)
        Dim oneview As Integer = freespace / 2

        Dim y As Integer = Me.Height - 60

        ListView1.Width = oneview
        ListView1.Left = Panel2.Left + Panel2.Width + 10
        ListView2.Width = oneview
        ListView2.Left = ListView1.Left + ListView1.Width + 10

        ListView1.Height = y
        ListView2.Height = y

        Return True
    End Function

    Private Function MoveDir(source As String, destination As String)
        'Klasör taşıma
        Try
            If Not source = vbNullString Then
                Dim name As String = source.Remove(0, source.LastIndexOf("\") + 1)
                Dim dest As String = (destination + name)

                Directory.Move(source, dest)
            End If
        Catch ex As Exception
        End Try
        Return True
    End Function

    Private Function DeleteMode(type As Integer)
        'Silme işlemleri
        If type = 1 Then
            Try
                File.Delete(CPath + ListBox1.SelectedItem.ToString + "\" + ListView1.FocusedItem.Text)
            Catch ex As Exception
                MsgBox("Couldn't deleted the file..", MsgBoxStyle.Critical)
            End Try
        ElseIf type = 2 Then
            Try
                Directory.Delete(CPath + ListBox1.SelectedItem.ToString + "\" + ListView2.FocusedItem.Text)
            Catch ex As Exception
                MsgBox("Couldn't deleted the folder..", MsgBoxStyle.Critical)
            End Try
        ElseIf type = 3 Then
            Try
                Directory.Delete(CPath + ListBox1.SelectedItem.ToString, SearchOption.AllDirectories)
                ListView1.Items.Clear()
                imageList1.Images.Clear()
                ListView2.Items.Clear()
            Catch ex As Exception
                MsgBox("Couldn't deleted the category..", MsgBoxStyle.Critical)
            End Try
        End If

        Return True
    End Function

    Private Function FullScreen()
        'TamEkran yapma
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
        Else
            Me.WindowState = FormWindowState.Maximized
        End If
        Stretch()

        Return True
    End Function

    Private Function OpenCategory(type As Boolean)
        'Kategori sekmesini açma/kapatma
        If ToolStripMenuItem4.Text = 1 Then
            If Panel2.Width = 50 And Not type = False Then
                CategorySlide = True
            ElseIf Panel2.Width = 200 And Not type = True Then
                CategorySlide = False
            End If
            Timer1.Enabled = True
        End If

        Return True
    End Function

    Private Function DataBaseCheck()
        'DataBase check
        Try
            If Not My.Computer.FileSystem.DirectoryExists(MPath) Then
                My.Computer.FileSystem.CreateDirectory(MPath)
            End If
            If Not My.Computer.FileSystem.DirectoryExists(CPath) Then
                My.Computer.FileSystem.CreateDirectory(CPath)
            End If

            Timer3.Enabled = True
        Catch ex As Exception
            MsgBox("Cannot access to the application data location..")
            Application.Exit()
        End Try

        Return True
    End Function

    Public Function FName(path As String)
        'dosya ismini al
        Return System.IO.Path.GetFileName(path)
    End Function

'------------------------------------------

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Veri konumu açma
        Try
            Process.Start(CPath)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Kategori konumu açma
        Try
            Process.Start(CPath + ListBox1.SelectedItem.ToString)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Silme modunu devre dışı bırakma
        DelStat = "disabled"
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'Silme butonu
        DeleteMode(DelMode)
    End Sub

'------------------------------------------

    Private Sub Panel2_MouseEnter(sender As Object, e As EventArgs) Handles Panel2.MouseEnter
        'Panel2 Mouse giriş
        OpenCategory(True)
    End Sub

    Private Sub Panel2_MouseLeave(sender As Object, e As EventArgs) Handles Panel2.MouseLeave
        'Panel2 Mouse çıkış
        OpenCategory(False)
    End Sub

'------------------------------------------

    Private Sub ListView1_DragEnter(sender As Object, e As DragEventArgs) Handles ListView1.DragEnter
        'Listview1 dosya - drag enter
        If e.Data.GetDataPresent(DataFormats.FileDrop, False) = True Then
            e.Effect = DragDropEffects.All
        End If
    End Sub

    Private Sub ListView1_DragDrop(sender As Object, e As DragEventArgs) Handles ListView1.DragDrop
        'Listview1 file - dragdrop
        Dim DroppedItems As String() = e.Data.GetData(DataFormats.FileDrop)

        For Each f In DroppedItems
            Dim myfile As String = f

            FName(myfile).ToString.Remove(FName(myfile).ToString.LastIndexOf("."))
            System.IO.File.Move(myfile, CPath + ListBox1.SelectedItem.ToString + "\" + FName(myfile))
        Next
    End Sub

    Private Sub ListView2_DragEnter(sender As Object, e As DragEventArgs) Handles ListView2.DragEnter
        'listview2 klasör - dragenter
        If e.Data.GetDataPresent(DataFormats.FileDrop, False) = True Then
            e.Effect = DragDropEffects.All
        End If
    End Sub

    Private Sub ListView2_DragDrop(sender As Object, e As DragEventArgs) Handles ListView2.DragDrop
        'listview2 klasör - dragdrop
        Dim DroppedItems As String() = e.Data.GetData(DataFormats.FileDrop)

        For Each f In DroppedItems
            Dim myfile As String = f
            Dim myname As String = f.Split("\").Last

            System.IO.Directory.Move(myfile, CPath + ListBox1.SelectedItem.ToString + "\" + myname)
        Next
    End Sub

'------------------------------------------

    Private Sub AddFolderAsCategoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFolderAsCategoryToolStripMenuItem.Click
        'Belirli klasörü kategori olarak ekleme
        Dim fb As New FolderBrowserDialog
        fb.ShowDialog()
        MoveDir(fb.SelectedPath.ToString, CPath)

    End Sub

    Private Sub AddFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFileToolStripMenuItem.Click
        'Dosya ekleme
        Dim fb As New OpenFileDialog
        fb.ShowDialog()

        Try
            If fb.CheckFileExists = True Then
                Dim name As String = (fb.FileName).Remove(0, fb.FileName.LastIndexOf("\") + 1)
                File.Move(fb.FileName, CPath + ListBox1.SelectedItem.ToString + "\" + name)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub CreateCategoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateCategoryToolStripMenuItem.Click
        'Kategori oluşturma
        Dim name As String
        name = InputBox("Enter a category name: ")
        Try
            If Not name = vbNullString Then
                Directory.CreateDirectory(CPath + name)
            End If
        Catch ex As Exception
            MsgBox("Couldn't create the category.. (Enter a valid category name or give permission to the app.)", MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub CreateFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateFolderToolStripMenuItem.Click
        'Klasör oluşturma
        Dim name As String
        name = InputBox("Enter a category name: ")
        Try
            If Not name = vbNullString Then
                Directory.CreateDirectory(CPath + ListBox1.SelectedItem.ToString + "\" + name)
            End If
        Catch ex As Exception
            MsgBox("Couldn't create the category.. (Enter a valid folder name or give permission to the app.)", MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub AddFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddFolderToolStripMenuItem.Click
        'Klasör ekleme
        Dim fb As New FolderBrowserDialog
        fb.ShowDialog()
        MoveDir(fb.SelectedPath.ToString, CPath + ListBox1.SelectedItem.ToString + "\")
    End Sub

    Private Sub FileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FileToolStripMenuItem.Click
        'Dosya silme
        DelMode = 1
        DelStat = "enabled"
        Label5.Text = "Select a file to remove"
    End Sub

    Private Sub FolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FolderToolStripMenuItem.Click
        'Klasör silme
        DelMode = 2
        DelStat = "enabled"
        Label5.Text = "Select a folder to remove"
    End Sub

    Private Sub CategoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CategoryToolStripMenuItem.Click
        'Kategori silme
        DelMode = 3
        DelStat = "enabled"
        Label5.Text = "Select a category to remove"
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        'Kategori sabitleme
        Dim item As ToolStripMenuItem = ToolStripMenuItem4

        If Not CategoryMode = 3 Then
            CategoryMode = CategoryMode + 1
        Else
            CategoryMode = 1
        End If

        item.Text = CategoryMode
    End Sub

    Private Sub ToolStripMenuItem6_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem6.Click
        'Ayalar butonu
        Me.Hide()
        Form2.Show()
    End Sub

'------------------------------------------

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        'Open file
        Try
            If Not ListView1.FocusedItem.Index < 0 Then

                Process.Start(CPath + ListBox1.Text + "\" + ListView1.FocusedItem.Text)

            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub ListView2_DoubleClick(sender As Object, e As EventArgs) Handles ListView2.DoubleClick
        'Open folder
        Try
            If Not ListView2.FocusedItem.Index < 0 Then
                Process.Start(CPath + ListBox1.SelectedItem.ToString + "\" + ListView2.FocusedItem.Text)
            End If
        Catch ex As Exception
        End Try

    End Sub

End Class
