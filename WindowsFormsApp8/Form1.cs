using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp8;

namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        private readonly AppDbContext _db = new AppDbContext();
        private int _selectedGradeId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SeedInitialData();
            LoadComboBoxes();
            LoadGrades();
            CalculateStudentAverage();
        }

        private void SeedInitialData()
        {
            if (!_db.Students.Any())
            {
                _db.Students.AddRange(new[]
                {
                    new Student { FullName = "Əli Məmmədov" },
                    new Student { FullName = "Aysel Həsənova" },
                    new Student { FullName = "Kamran Quliyev" }
                });
            }

            if (!_db.Subjects.Any())
            {
                _db.Subjects.AddRange(new[]
                {
                    new Subject { Name = "Proqramlaşdırma" },
                    new Subject { Name = "Verilənlər Bazası" },
                    new Subject { Name = "Riyaziyyat" }
                });
            }
            _db.SaveChanges();
        }

        private void LoadComboBoxes()
        {
            cmbStudent.DisplayMember = "FullName";
            cmbStudent.ValueMember = "Id";
            cmbStudent.DataSource = _db.Students.ToList();

            cmbSubject.DisplayMember = "Name";
            cmbSubject.ValueMember = "Id";
            cmbSubject.DataSource = _db.Subjects.ToList();
        }

        private void LoadGrades()
        {
            var gradeList = _db.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Select(g => new
                {
                    Id = g.Id,
                    Telebe = g.Student.FullName,
                    Fenn = g.Subject.Name,
                    Qiymet = g.Score,
                    StudentId = g.StudentId,
                    SubjectId = g.SubjectId
                })
                .ToList();

            dgvGrades.DataSource = gradeList;

            if (dgvGrades.Columns["StudentId"] != null)
                dgvGrades.Columns["StudentId"].Visible = false;
            if (dgvGrades.Columns["SubjectId"] != null)
                dgvGrades.Columns["SubjectId"].Visible = false;
        }

        private void CalculateStudentAverage()
        {
            if (cmbStudent.SelectedValue is int studentId)
            {
                var studentGrades = _db.Grades.Where(g => g.StudentId == studentId);

                if (studentGrades.Any())
                {
                    double avg = studentGrades.Average(g => g.Score);
                    label1.Text = $"Orta bal: {avg:F2}";
                }
                else
                {
                    label1.Text = "Qiymət yoxdur";
                }
            }
        }

        private void cmbStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateStudentAverage();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateScore(out int score)) return;

            var newGrade = new Grade
            {
                StudentId = (int)cmbStudent.SelectedValue,
                SubjectId = (int)cmbSubject.SelectedValue,
                Score = score
            };

            _db.Grades.Add(newGrade);
            _db.SaveChanges();

            LoadGrades();
            CalculateStudentAverage();
            txtScore.Clear();
            MessageBox.Show("Qiymət əlavə edildi.");
        }

        private void dgvGrades_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvGrades.Rows[e.RowIndex];
                _selectedGradeId = Convert.ToInt32(row.Cells["Id"].Value);
                cmbStudent.SelectedValue = Convert.ToInt32(row.Cells["StudentId"].Value);
                cmbSubject.SelectedValue = Convert.ToInt32(row.Cells["SubjectId"].Value);
                txtScore.Text = row.Cells["Qiymet"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedGradeId == 0)
            {
                MessageBox.Show("Yeniləmək üçün cədvəldən sətir seçin.");
                return;
            }

            if (!ValidateScore(out int score)) return;

            var grade = _db.Grades.Find(_selectedGradeId);
            if (grade != null)
            {
                grade.StudentId = (int)cmbStudent.SelectedValue;
                grade.SubjectId = (int)cmbSubject.SelectedValue;
                grade.Score = score;

                _db.SaveChanges();
                LoadGrades();
                CalculateStudentAverage();
                _selectedGradeId = 0;
                txtScore.Clear();
                MessageBox.Show("Qiymət yeniləndi.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedGradeId == 0)
            {
                MessageBox.Show("Silmək üçün cədvəldən sətir seçin.");
                return;
            }

            var grade = _db.Grades.Find(_selectedGradeId);
            if (grade != null)
            {
                _db.Grades.Remove(grade);
                _db.SaveChanges();

                LoadGrades();
                CalculateStudentAverage();
                _selectedGradeId = 0;
                txtScore.Clear();
                MessageBox.Show("Qiymət silindi.");
            }
        }

        private bool ValidateScore(out int score)
        {
            if (!int.TryParse(txtScore.Text, out score) || score < 0 || score > 100)
            {
                MessageBox.Show("Zəhmət olmasa 0 ilə 100 arasında düzgün qiymət daxil edin.");
                return false;
            }
            return true;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}