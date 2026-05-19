using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nasag.Models;

namespace Nasag.Data;

public sealed class DbSeeder : IDbSeeder
{
    private readonly IDbContextFactory<NasaqDbContext> _factory;

    public DbSeeder(IDbContextFactory<NasaqDbContext> factory)
    {
        _factory = factory;
    }

    public async Task SeedIfEmptyAsync(CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct).ConfigureAwait(false);

        if (await ctx.Users.AnyAsync(ct).ConfigureAwait(false))
            return;

        // 1) Roles
        var adminRole = new Role { NameAr = "مدير النظام", Permissions = Permission.All, IsSystem = true };
        var principalRole = new Role
        {
            NameAr = "مدير المدرسة",
            Permissions = Permission.All & ~(Permission.ManageUsers | Permission.ManageBackup)
        };
        var teacherRole = new Role
        {
            NameAr = "معلم",
            Permissions = Permission.ViewDashboard | Permission.ManageAttendance
                        | Permission.ManageMarks | Permission.ViewResults
        };
        var accountantRole = new Role
        {
            NameAr = "محاسب",
            Permissions = Permission.ViewDashboard | Permission.ManageFees | Permission.ManageReports
        };
        ctx.Roles.AddRange(adminRole, principalRole, teacherRole, accountantRole);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 2) Admin user
        var admin = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "مدير النظام",
            IsActive = true,
            RoleId = adminRole.Id,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Users.Add(admin);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 3) Academic year
        var year = new AcademicYear
        {
            NameAr = "2025 - 2026",
            StartDate = new DateTime(2025, 9, 1),
            EndDate = new DateTime(2026, 6, 30),
            IsActive = true
        };
        ctx.AcademicYears.Add(year);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 4) School settings
        ctx.SchoolSettings.Add(new SchoolSettings
        {
            NameAr = "مدرسة النور الأهلية",
            Phone = "0112345678",
            Email = "info@alnoor-school.edu",
            Address = "الرياض - حي النخيل",
            PrincipalName = "أ. عبدالله الفهد",
            CurrentAcademicYearId = year.Id
        });
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 5) Grades (primary 1-6, middle 1-3, high 1-3)
        var grades = new List<Grade>
        {
            new() { NameAr = "الأول الابتدائي",   Level = GradeLevel.Primary, SortOrder = 1 },
            new() { NameAr = "الثاني الابتدائي",  Level = GradeLevel.Primary, SortOrder = 2 },
            new() { NameAr = "الثالث الابتدائي",  Level = GradeLevel.Primary, SortOrder = 3 },
            new() { NameAr = "الرابع الابتدائي",  Level = GradeLevel.Primary, SortOrder = 4 },
            new() { NameAr = "الخامس الابتدائي",  Level = GradeLevel.Primary, SortOrder = 5 },
            new() { NameAr = "السادس الابتدائي",  Level = GradeLevel.Primary, SortOrder = 6 },
            new() { NameAr = "الأول المتوسط",     Level = GradeLevel.Middle,  SortOrder = 7 },
            new() { NameAr = "الثاني المتوسط",    Level = GradeLevel.Middle,  SortOrder = 8 },
            new() { NameAr = "الثالث المتوسط",    Level = GradeLevel.Middle,  SortOrder = 9 },
            new() { NameAr = "الأول الثانوي",     Level = GradeLevel.High,    SortOrder = 10 },
            new() { NameAr = "الثاني الثانوي",    Level = GradeLevel.High,    SortOrder = 11 },
            new() { NameAr = "الثالث الثانوي",    Level = GradeLevel.High,    SortOrder = 12 }
        };
        ctx.Grades.AddRange(grades);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 6) Sections — اقتصر على شعب لأول 4 صفوف لتسريع البذرة، مع شعبتين لكل صف
        var sectionLetters = new[] { "أ", "ب" };
        var sections = new List<Section>();
        foreach (var g in grades.Take(6))
        {
            foreach (var letter in sectionLetters)
            {
                sections.Add(new Section
                {
                    NameAr = letter,
                    Capacity = 30,
                    GradeId = g.Id,
                    AcademicYearId = year.Id
                });
            }
        }
        ctx.Sections.AddRange(sections);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 7) Subjects — لكل صف من أول 6 صفوف نفس المواد الأساسية
        var subjectNames = new[]
        {
            "اللغة العربية", "الرياضيات", "العلوم", "الدراسات الاجتماعية",
            "اللغة الإنجليزية", "التربية الإسلامية"
        };
        var subjects = new List<Subject>();
        foreach (var g in grades.Take(6))
        {
            foreach (var name in subjectNames)
                subjects.Add(new Subject { NameAr = name, GradeId = g.Id, MaxMark = 100m, PassMark = 50m });
        }
        ctx.Subjects.AddRange(subjects);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 8) Exams
        var exams = new[]
        {
            new Exam { NameAr = "الشهري الأول",  Weight = 1m, AcademicYearId = year.Id },
            new Exam { NameAr = "الفصلي",        Weight = 2m, AcademicYearId = year.Id },
            new Exam { NameAr = "النهائي",       Weight = 3m, AcademicYearId = year.Id }
        };
        ctx.Exams.AddRange(exams);

        // 9) Fee plans (واحد لكل صف من أول 6 صفوف)
        var feePlans = new List<FeePlan>();
        var idx = 0;
        foreach (var g in grades.Take(6))
        {
            feePlans.Add(new FeePlan
            {
                NameAr = $"رسوم {g.NameAr} - {year.NameAr}",
                TotalAmount = 5000m + idx * 500m,
                InstallmentsCount = 4,
                GradeId = g.Id,
                AcademicYearId = year.Id
            });
            idx++;
        }
        ctx.FeePlans.AddRange(feePlans);
        await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

        // 10) Guardians + Students (~30 student demo set)
        var maleFirstNames = new[] { "أحمد", "محمد", "عبدالله", "خالد", "يوسف", "سالم", "ياسين", "بدر", "زياد", "عمر", "حسن", "إبراهيم", "فهد", "مازن", "علي" };
        var femaleFirstNames = new[] { "نور", "سارة", "فاطمة", "ليلى", "مريم", "هدى", "ريم", "لمى", "جنى", "روان", "آلاء", "هيا", "تالة", "ميس", "دانة" };
        var familyNames = new[] { "العتيبي", "القحطاني", "السبيعي", "الدوسري", "الحربي", "الزهراني", "الشمري", "الغامدي", "العنزي", "المالكي" };

        var rng = new Random(42);
        var studentNumberSeq = 20250001;

        // Track (StudentFee, its Installments) per student so we can layer
        // demo payments after all rows exist.
        var seededFees = new List<(StudentFee Fee, List<Installment> Installments)>();

        for (var i = 0; i < 30; i++)
        {
            var isMale = i % 2 == 0;
            var first = isMale ? maleFirstNames[rng.Next(maleFirstNames.Length)] : femaleFirstNames[rng.Next(femaleFirstNames.Length)];
            var family = familyNames[rng.Next(familyNames.Length)];
            var fatherFirst = maleFirstNames[rng.Next(maleFirstNames.Length)];

            var guardian = new Guardian
            {
                FullName = $"{fatherFirst} {family}",
                Relation = GuardianRelation.Father,
                Phone = "05" + rng.Next(10000000, 99999999).ToString(),
                NationalId = "1" + rng.Next(100000000, 999999999).ToString(),
                Occupation = "موظف",
                Address = "الرياض"
            };
            ctx.Guardians.Add(guardian);
            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            var section = sections[rng.Next(sections.Count)];
            var student = new Student
            {
                StudentNumber = (studentNumberSeq++).ToString(),
                FullName = $"{first} {fatherFirst} {family}",
                Gender = isMale ? Gender.Male : Gender.Female,
                BirthDate = new DateTime(2015 - rng.Next(0, 6), rng.Next(1, 13), rng.Next(1, 28)),
                EnrollmentDate = new DateTime(2025, 9, 1),
                Status = StudentStatus.Active,
                SectionId = section.Id,
                GuardianId = guardian.Id,
                NationalId = "1" + rng.Next(100000000, 999999999).ToString(),
                Phone = guardian.Phone
            };
            ctx.Students.Add(student);
            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            // Attach a StudentFee row matching that grade's plan
            var plan = feePlans.First(p => p.GradeId == section.GradeId);
            var fee = new StudentFee
            {
                StudentId = student.Id,
                FeePlanId = plan.Id,
                TotalAmount = plan.TotalAmount,
                PaidAmount = 0m
            };
            ctx.StudentFees.Add(fee);
            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            var installmentAmount = Math.Round(plan.TotalAmount / plan.InstallmentsCount, 2);
            var due = new DateTime(2025, 9, 15);
            var feeInstallments = new List<Installment>();
            for (var n = 1; n <= plan.InstallmentsCount; n++)
            {
                var inst = new Installment
                {
                    StudentFeeId = fee.Id,
                    Number = n,
                    Amount = installmentAmount,
                    DueDate = due.AddMonths((n - 1) * 2),
                    Status = InstallmentStatus.Due
                };
                ctx.Installments.Add(inst);
                feeInstallments.Add(inst);
            }
            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            seededFees.Add((fee, feeInstallments));
        }

        // 11) Demo payments — pick 10-12 students from the first 30 and add
        //     1-2 partial payments per student (30-50% of one installment).
        //     Wrapped in a single ExecutionStrategy-compatible transaction to
        //     keep payment+installment+fee updates atomic.
        var paymentRng = new Random(20260519);
        var receiptCounter = 1;
        var receiptDate = new DateTime(2025, 10, 1);

        var paymentTargetCount = paymentRng.Next(10, 13); // 10..12
        var candidateIndexes = Enumerable.Range(0, seededFees.Count)
            .OrderBy(_ => paymentRng.Next())
            .Take(paymentTargetCount)
            .ToList();

        var strategy = ctx.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            foreach (var idxFee in candidateIndexes)
            {
                var (fee, installments) = seededFees[idxFee];
                if (installments.Count == 0) continue;

                var paymentCount = paymentRng.Next(1, 3); // 1..2
                for (var k = 0; k < paymentCount; k++)
                {
                    // Apply to the earliest still-unfinished installment.
                    var target = installments.FirstOrDefault(i => i.PaidAmount < i.Amount - 0.01m);
                    if (target is null) break;

                    var ratio = 0.30 + paymentRng.NextDouble() * 0.20; // 0.30..0.50
                    var amount = Math.Round(target.Amount * (decimal)ratio, 2);
                    if (amount <= 0m) continue;
                    // Never overshoot a single installment in seed data.
                    var remainingOnInst = target.Amount - target.PaidAmount;
                    if (amount > remainingOnInst) amount = remainingOnInst;

                    var receipt = new Payment
                    {
                        ReceiptNumber = $"REC-{receiptDate:yyyyMMdd}-{receiptCounter:0000}",
                        Amount = amount,
                        PaymentDate = receiptDate,
                        Method = PaymentMethod.Cash,
                        StudentFeeId = fee.Id,
                        InstallmentId = target.Id,
                        UserId = admin.Id,
                        Notes = "بذرة تجريبية"
                    };
                    ctx.Payments.Add(receipt);

                    target.PaidAmount += amount;
                    fee.PaidAmount += amount;

                    target.Status = target.PaidAmount >= target.Amount - 0.01m
                        ? InstallmentStatus.Paid
                        : target.PaidAmount > 0m
                            ? InstallmentStatus.PartiallyPaid
                            : InstallmentStatus.Due;

                    receiptCounter++;
                    receiptDate = receiptDate.AddDays(paymentRng.Next(1, 6));
                }
            }

            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);

            // 12) Overdue: any past-due installment that hasn't been paid at all.
            var today = DateTime.Today;
            foreach (var (_, installments) in seededFees)
            {
                foreach (var inst in installments)
                {
                    if (inst.Status == InstallmentStatus.Due
                        && inst.PaidAmount == 0m
                        && inst.DueDate < today)
                    {
                        inst.Status = InstallmentStatus.Overdue;
                    }
                }
            }

            await ctx.SaveChangesAsync(ct).ConfigureAwait(false);
            await tx.CommitAsync(ct).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
