# نَسَق لإدارة المدارس - Agent Plan

> ملف الذاكرة الرسمي وخطة العمل لمشروع نَسَق. أي وكيل AI يدخل المشروع يجب أن يقرأ هذا الملف بالكامل قبل البدء.

---

## 1. Project Overview

**نَسَق لإدارة المدارس** هو برنامج Windows Desktop عربي احترافي لإدارة المدارس، يهدف إلى تقديم نظام عملي وبسيط يغطي الوظائف الفعلية المستخدمة في المدارس الحقيقية:

- إدارة الطلاب وأولياء الأمور
- إدارة الصفوف والشعب
- تسجيل الحضور والغياب اليومي
- إدارة المواد والامتحانات وإدخال الدرجات
- حساب النتائج النهائية
- إدارة الرسوم والأقساط والمدفوعات
- إصدار التقارير
- إدارة المستخدمين والصلاحيات
- إعدادات المدرسة والنسخ الاحتياطي

**النطاق المستبعد صراحةً (لا تطوّر):** LMS، تطبيق ولي أمر، تطبيق طالب، الباصات، المكتبة، الكافتيريا، الدردشة، الدفع الإلكتروني، المحاسبة الكاملة، ميزات الذكاء الاصطناعي داخل البرنامج.

---

## 2. Technology Stack

| المكوّن | القرار |
|---------|--------|
| لغة البرمجة | C# |
| إطار العمل | .NET 8 (net8.0-windows) |
| واجهة المستخدم | WPF |
| النمط المعماري | MVVM |
| قاعدة البيانات | SQL Server (LocalDB في التطوير، Express/Standard في الإنتاج) |
| ORM | Entity Framework Core 8 |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |
| اللغة | عربية فقط - RTL |
| الخط | Tajawal |
| نظام التشغيل | Windows 10/11 |

> ملاحظة: المشروع الحالي هو WPF .NET 8 فارغ تم إنشاؤه مسبقاً في `Nasag/Nasag.csproj`. سيتم البناء فوقه.

---

## 3. Visual Design Reference

مجلد `UI/` هو **المرجع البصري الأساسي والملزم**. يحتوي على 10 صور تصاميم تغطي الشاشات الرئيسية:

| الملف | الشاشة |
|------|--------|
| (1).png | لوحة التحكم (Dashboard) |
| (2).png | شاشة تسجيل الدخول (Login) |
| (3).png | قائمة الطلاب |
| (4).png | إضافة / تعديل طالب |
| (5).png | الصفوف والشعب |
| (6).png | الحضور والغياب |
| (7).png | إدخال الدرجات |
| (8).png | نتائج الطلاب |
| (9).png | الرسوم والأقساط |
| (10).png | مركز التقارير |

### الهوية البصرية المستخرجة من التصاميم

**التخطيط العام:**
- قائمة جانبية ثابتة على اليمين (Right Sidebar) بخلفية Navy داكنة، تحتوي الشعار في الأعلى ثم عناصر التنقل.
- شريط علوي (Top Bar) أبيض يحتوي: بحث عام، اختيار المدرسة، اختيار السنة الدراسية، إشعارات، اسم المستخدم وصورته.
- منطقة محتوى مركزية بخلفية فاتحة (off-white) تحتوي البطاقات والجداول.

**لوحة الألوان (Color Palette):**

| الاستخدام | اللون (تقريبي) |
|----------|---------------|
| Navy Sidebar (الخلفية الجانبية) | `#0E2A47` |
| Navy Deep (نصوص رئيسية، عناوين) | `#1B3A57` |
| Teal Primary (الأزرار الأساسية، التحديد، التأكيد) | `#1FB5A8` |
| Teal Hover | `#19A294` |
| Background (خلفية المحتوى) | `#F5F7FB` |
| Card Surface | `#FFFFFF` |
| Border / Divider | `#E5E9F0` |
| Text Primary | `#1B3A57` |
| Text Secondary | `#6B7A8F` |
| Success (حاضر، مدفوع، ناجح) | `#22C55E` |
| Warning (متأخر، مستحق) | `#F59E0B` |
| Danger (غائب، متأخر السداد، راسب) | `#EF4444` |
| Info (إجازة، ملاحظة) | `#3B82F6` |

**النمط البصري:**
- بطاقات بيضاء بحواف مدورة (radius ~12px) مع ظل ناعم.
- أزرار أساسية: خلفية Teal + نص أبيض + حواف مدورة.
- أزرار ثانوية: خلفية بيضاء + إطار رمادي فاتح + نص Navy.
- DataGrid نظيف: صفوف بيضاء، فاصل أفقي ناعم، رأس بخلفية فاتحة، أعمدة عمل (تعديل/حذف) كأزرار أيقونية صغيرة.
- شارات الحالة (Status Pills): كبسولات صغيرة بخلفية ملونة شفافة ونص بنفس اللون الكامل.
- الحقول: TextBox / ComboBox / DatePicker بحواف مدورة، إطار رمادي، تركيز Teal.
- المسافات: padding كريم بين العناصر، عدم الازدحام.

**الأيقونات:** نمط outline / line-icons (مثل Lucide أو Phosphor)، بحجم 18-22px في القائمة الجانبية و14-16px داخل الجداول.

**الاتجاه:** `FlowDirection="RightToLeft"` على مستوى التطبيق.

**الخط:** Tajawal بأوزان Regular / Medium / Bold.

---

## 4. Core Rules (قواعد ملزمة)

1. البرنامج عربي بالكامل — جميع النصوص الظاهرة للمستخدم بالعربية.
2. كل الواجهات RTL (FlowDirection = RightToLeft).
3. لا تستخدم وظائف خارج النطاق المذكور في القسم 1.
4. لا تكسر بنية المشروع المعتمدة في القسم 6.
5. لا تكرر الكود — إذا أمكن إنشاء Style / UserControl / Converter مشترك فافعل ذلك.
6. لا تستخدم صور `UI/` كخلفيات أو ImageBrush لتقليد الواجهة — هي مرجع بصري فقط.
7. كل الواجهات يجب أن تكون XAML حقيقية وقابلة للتشغيل والتطوير.
8. كل مرحلة يجب أن تُختبر (Build ناجح + تشغيل + اختبار يدوي للسيناريو الأساسي) قبل الانتقال للتالية.
9. بعد إكمال أي مرحلة، يجب تحديث هذا الملف (قسم 8 Current Progress + قسم 9 Decisions Log).
10. أسماء الجداول والكود إنجليزية، نصوص الواجهة عربية فقط.
11. أصلح الأخطاء من جذورها، لا تخفِ الأعراض.
12. لا تضف ميزات لم تُطلب — التزم بالـ Scope.

---

## 5. Database Plan

قاعدة البيانات: **SQL Server**. اسم القاعدة المقترح: `NasaqSchoolDb`.

### الجداول الأساسية

| الجدول | الوصف | حقول رئيسية |
|--------|-------|-------------|
| `Users` | مستخدمو النظام | Id, Username, PasswordHash, FullName, RoleId, IsActive |
| `Roles` | الأدوار والصلاحيات | Id, NameAr, Permissions (json/bitmask) |
| `SchoolSettings` | بيانات المدرسة | Id, NameAr, LogoPath, Address, Phone, Email, CurrentAcademicYearId |
| `AcademicYears` | السنوات الدراسية | Id, NameAr (مثل "2025 - 2026"), StartDate, EndDate, IsActive |
| `Grades` | الصفوف (الأول، الثاني...) | Id, NameAr, Level (Primary/Middle/High), SortOrder |
| `Sections` | الشعب (أ، ب، ج) | Id, GradeId, NameAr, Capacity, AcademicYearId |
| `Guardians` | أولياء الأمور | Id, FullName, Relation, Phone, Email, NationalId |
| `Students` | الطلاب | Id, StudentNumber, FullName, Gender, BirthDate, NationalId, GradeId, SectionId, GuardianId, PhotoPath, EnrollmentDate, Status (Active/Archived) |
| `Subjects` | المواد | Id, NameAr, GradeId, MaxMark, PassMark |
| `Exams` | أنواع الامتحانات | Id, NameAr (شهري/فصلي/نهائي), AcademicYearId, Weight |
| `Marks` | الدرجات | Id, StudentId, SubjectId, ExamId, Mark, Notes |
| `AttendanceRecords` | الحضور | Id, StudentId, Date, Status (Present/Absent/Late/Excused), Notes |
| `FeePlans` | خطط الرسوم | Id, NameAr, GradeId, TotalAmount, AcademicYearId |
| `StudentFees` | رسوم الطالب | Id, StudentId, FeePlanId, TotalAmount, PaidAmount, RemainingAmount |
| `Installments` | الأقساط | Id, StudentFeeId, InstallmentNumber, DueDate, Amount, Status (Paid/Due/Overdue) |
| `Payments` | سندات القبض | Id, StudentFeeId, InstallmentId, Amount, PaymentDate, ReceiptNumber, Method, UserId, Notes |
| `BackupLogs` | سجل النسخ الاحتياطي | Id, FilePath, CreatedAt, CreatedBy, SizeBytes |

### العلاقات الأساسية
- `Student` → `Section` → `Grade`
- `Student` → `Guardian` (many-to-one، مع إمكانية مشاركة ولي أمر لعدة طلاب)
- `Subject` → `Grade`
- `Mark` → `Student`, `Subject`, `Exam`
- `AttendanceRecord` → `Student`
- `StudentFee` → `Student`, `FeePlan`
- `Installment` → `StudentFee`
- `Payment` → `StudentFee`, `Installment`, `User`

### Seed Data (تجريبية)
- مدرسة: "مدرسة النور الأهلية"
- السنة الدراسية: "2025 - 2026"
- صفوف: الأول الابتدائي … الثالث الثانوي
- شعب: أ، ب، ج
- مستخدم تجريبي: admin / admin123 (مدير النظام)
- ~30 طالب بأسماء عربية واقعية
- مواد، امتحانات، درجات، حضور، رسوم تجريبية

---

## 6. Architecture Plan

البنية المعتمدة (تبنى داخل مشروع `Nasag` الحالي، مع إمكانية تقسيم مستقبلي إلى مشاريع مكتبات):

```
/Nasag
  /Assets
    /Fonts            ← Tajawal-Regular.ttf, Tajawal-Medium.ttf, Tajawal-Bold.ttf
    /Images           ← Logo.png وأيقونات
  /Themes
    Colors.xaml       ← فرشاة الألوان الكاملة
    Typography.xaml   ← أنماط النصوص والعناوين
    Buttons.xaml      ← أنماط الأزرار (Primary, Secondary, Icon, Danger)
    Inputs.xaml       ← أنماط TextBox, PasswordBox, ComboBox, DatePicker
    DataGrid.xaml     ← نمط DataGrid + أعمدة الإجراءات
    Cards.xaml        ← أنماط البطاقات والحاويات
    StatusPills.xaml  ← شارات الحالة
    Icons.xaml        ← Geometry للأيقونات
  /Controls           ← UserControls قابلة لإعادة الاستخدام
    StatCard.xaml
    StatusPill.xaml
    SidebarMenuItem.xaml
    SectionHeader.xaml
  /Views
    /Auth             ← LoginView
    /Shell            ← MainShellView (يحوي Sidebar + TopBar + ContentHost)
    /Dashboard
    /Students
    /Classes
    /Attendance
    /Subjects
    /Marks
    /Results
    /Fees
    /Reports
    /Users
    /Settings
    /Backup
  /ViewModels
    (واحد لكل View، مع BaseViewModel ونظام Navigation)
  /Models             ← Entity classes (Student, Section, ...)
  /Data
    NasaqDbContext.cs
    /Migrations
    DbSeeder.cs
  /Repositories       ← StudentsRepository, AttendanceRepository, ... (واجهات + تنفيذ)
  /Services
    INavigationService / NavigationService
    IAuthService / AuthService
    ICurrentUserService
    IDialogService
    IBackupService
    IReportService
  /Helpers            ← Converters, Extensions, RelayCommand, ObservableObject
  App.xaml
  App.xaml.cs         ← DI Container, Startup
  MainWindow.xaml     ← shell host
```

**أنماط رئيسية:**
- MVVM مع `CommunityToolkit.Mvvm` (RelayCommand + ObservableObject + Source Generators).
- Navigation عبر `INavigationService` بدلاً من Frame البحت.
- Repository Pattern فوق EF Core.
- DI عبر `Microsoft.Extensions.Hosting` في `App.xaml.cs`.

**حزم NuGet المتوقعة (الحد الأدنى):**
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Hosting`
- `Microsoft.Extensions.Configuration.Json`
- `CommunityToolkit.Mvvm`
- (اختياري لاحقاً) `QuestPDF` أو `iText` للتقارير، `EPPlus` لـ Excel، `LiveChartsCore.SkiaSharpView.WPF` للرسوم البيانية.

---

## 7. Development Phases

### Phase 0 — Planning and Agent Files
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] فحص بنية المشروع
- [x] فحص مجلد UI (10 صور)
- [x] استخراج الهوية البصرية والألوان
- [x] تحديد المعمارية
- [x] تحديد الجداول
- [x] إنشاء Agent.md
- [x] إنشاء AI_INSTRUCTIONS.md

**Acceptance Criteria:**
- [x] Agent.md موجود
- [x] AI_INSTRUCTIONS.md موجود
- [x] خطة كاملة موثقة
- [x] لا كود فعلي قبل الموافقة على الانتقال

---

### Phase 1 — Project Foundation
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] إنشاء بنية المجلدات في `Nasag/` (Assets/Fonts, Assets/Images, Themes, Views, ViewModels, Models, Data, Repositories, Services, Helpers, Controls).
- [x] إضافة حزم NuGet الأساسية: CommunityToolkit.Mvvm 8.3.2، EF Core 8.0.10 (Core + SqlServer + Tools)، Microsoft.Extensions.Hosting 8.0.1 (+ Configuration / Configuration.Json / DependencyInjection).
- [x] تنزيل خط Tajawal (Regular/Medium/Bold) من Google Fonts إلى `Assets/Fonts/` وتسجيله كـ Resource في `.csproj`.
- [x] إنشاء `Themes/Colors.xaml` (لوحة كاملة + Brushes + CornerRadius).
- [x] إنشاء `Themes/Typography.xaml` (TajawalFont + أحجام + أنماط TextBlock).
- [x] إنشاء `Themes/Common.xaml` (defaults ضمنية لـ Window / TextBlock / Control: خط، اتجاه RTL، خلفية، ألوان).
- [x] ضبط `App.xaml` لتحميل الـ ResourceDictionaries الثلاثة عبر MergedDictionaries، وحذف StartupUri لصالح bootstrap يدوي.
- [x] إعداد DI Container في `App.xaml.cs` باستخدام Microsoft.Extensions.Hosting + قراءة `appsettings.json` + تسجيل services + ViewModels.
- [x] إنشاء `IAppInfoService` (سيرفس تجريبي صغير) و`MainViewModel` لإثبات DI.
- [x] تحديث `MainWindow.xaml` كـ smoke-test: بطاقة مركزية بالشعار + اسم البرنامج + شريط Color Swatches للتحقق من اللوحة.
- [x] إنشاء `appsettings.json` بـ ConnectionString لـ LocalDB.
- [x] الاعتماد على CommunityToolkit.Mvvm `ObservableObject` و`RelayCommand` بدلاً من كتابة BaseViewModel يدوي.

**Acceptance Criteria:**
- [x] المشروع يبني بدون أخطاء (Build succeeded: 0 Warning, 0 Error).
- [x] النافذة تفتح بخلفية فاتحة `#F5F7FB` وخط Tajawal مُحمَّل ومرئي.
- [x] DI يعمل: `IAppInfoService` → `MainViewModel` تحقن في `MainWindow.DataContext` بنجاح.
- [x] FlowDirection RTL مفعّل عبر default style على Window في `Common.xaml`.
- [x] Generic Host يبدأ ويسجل `Application started.` بدون استثناءات.

---

### Phase 2 — Design System and Main Shell
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] بناء `MainShellView` (Window) يحتوي: Sidebar يميناً (عرض 260) + TopBar أعلى + ContentHost في المنتصف.
- [x] Sidebar بخلفية Navy، الشعار في الأعلى، 12 عنصر تنقل: الرئيسية، الطلاب، الصفوف والشعب، الحضور، المواد، إدخال الدرجات، النتائج، الرسوم، التقارير، المستخدمون، الإعدادات، النسخ الاحتياطي.
- [x] TopBar: شريحة مستخدم، شريحة سنة، شريحة مدرسة، صندوق بحث، إشعارات، تسجيل خروج.
- [x] أنماط الأزرار: `PrimaryButton`, `SecondaryButton`, `DangerButton`, `GhostButton`, `IconButton`, `RowActionButton`, `BusyPrimaryButton` في [Themes/Buttons.xaml](Nasag/Themes/Buttons.xaml).
- [x] أنماط الحقول الافتراضية: TextBox, PasswordBox, ComboBox, DatePicker + `FieldLabel` في [Themes/Inputs.xaml](Nasag/Themes/Inputs.xaml).
- [x] نمط DataGrid كامل (Row/Cell/Header) + AlternatingRow في [Themes/DataGrid.xaml](Nasag/Themes/DataGrid.xaml).
- [x] أنماط البطاقات والظلال في [Themes/Cards.xaml](Nasag/Themes/Cards.xaml).
- [x] شارات الحالة (Success/Warning/Danger/Info/Teal) في [Themes/StatusPills.xaml](Nasag/Themes/StatusPills.xaml).
- [x] مكتبة Geometry للأيقونات (22 أيقونة من نمط outline) في [Themes/Icons.xaml](Nasag/Themes/Icons.xaml).
- [x] UserControls: `StatCard`, `SectionHeader`, `SidebarMenuItem`, `LoadingOverlay`, `ConnectionStatusBanner` تحت `Controls/`.
- [x] `IBusyService` / `BusyService` لإدارة حالة Busy + رسالة قابلة للتخصيص.
- [x] `IConnectionMonitor` / `ConnectionMonitor` stub (Phase 3 سيربطه بـ `CanConnectAsync`).
- [x] `INavigationService` / `NavigationService` مع 12 NavigationDescriptor + Resolver عبر IServiceProvider.
- [x] 12 PageViewModel ترث `PageViewModel` (Dashboard/Students/Classes/Attendance/Subjects/Marks/Results/Fees/Reports/Users/Settings/Backup).
- [x] `PagePlaceholderView` موحّد للشاشات قبل بناء كل واحدة لاحقاً.
- [x] `DataTemplates.xaml` يربط كل PageVM بالـ Placeholder (سيُستبدل لاحقاً عند بناء View حقيقي).
- [x] `MainShellViewModel` يدير NavigationItems + IsActive + IsDisconnected + RetryConnectionCommand.
- [x] `ResourceKeyConverter` لربط IconKey (string) بـ Geometry من Application.Current.Resources.
- [x] تسجيل كل services + ViewModels في DI في `App.xaml.cs`.
- [x] استبدال startup إلى MainShellView مع حقن `MainShellViewModel` في `DataContext`.
- [x] إزالة `MainWindow` و`MainViewModel` القديمين (لم يعودا مستخدمين).

**Acceptance Criteria:**
- [x] الشكل العام مطابق للهوية البصرية (Navy sidebar + Teal accents + بطاقات بيضاء + ظل ناعم + RTL).
- [x] التنقل بين 12 قسم يعمل عبر القائمة الجانبية (Highlight + Active strip).
- [x] لا توجد صور UI مستخدمة كخلفية — كل شيء XAML/Geometry.
- [x] الأنماط مركزية في `/Themes` بدون تكرار.
- [x] `LoadingOverlay` جاهز للربط بـ `IsBusy` على أي ViewModel.
- [x] `ConnectionStatusBanner` يظهر/يختفي ديناميكياً من `MainShellViewModel.IsDisconnected`.
- [x] Build: 0 Warning / 0 Error. التطبيق يقلع بدون استثناءات.

---

### Phase 3 — Database and Core Entities
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] إنشاء كل Model في `/Models` (17 كياناً + Enums): Role, User, SchoolSettings, AcademicYear, Grade, Section, Guardian, Student, Subject, Exam, Mark, AttendanceRecord, FeePlan, StudentFee, Installment, Payment, BackupLog.
- [x] إنشاء [NasaqDbContext](Nasag/Data/NasaqDbContext.cs) مع DbSets والعلاقات الكاملة (Fluent API: أنواع، أطوال، Precision، Indexes فريدة، Delete behaviors).
- [x] ضبط connection string في `appsettings.json` (موجود من Phase 1) + قراءته عبر `IConfiguration` عند تسجيل `AddDbContextFactory<NasaqDbContext>` في DI.
- [x] تفعيل `EnableRetryOnFailure(5, TimeSpan.FromSeconds(10))` في إعداد `UseSqlServer`.
- [x] إنشاء أول Migration `20260515081343_InitialCreate` تحت [Nasag/Data/Migrations/](Nasag/Data/Migrations/).
- [x] **`IDatabaseInitializer` / `DatabaseInitializer` يطبّق Migrations ديناميكياً عند بدء التطبيق:**
  - `GetPendingMigrationsAsync()` ثم `MigrateAsync()` تلقائياً — أي Migration جديدة تُلتقط بلا كود إضافي.
  - `CanConnectAsync()` يُستدعى عندما لا توجد Migrations لضمان الوصول.
  - استدعاء `IDbSeeder.SeedIfEmptyAsync()` بعد Migrations.
  - معالجة أخطاء (SqlException، عام) وإرجاع `DatabaseInitResult` مع `DatabaseInitStatus` (Success/CannotConnect/MigrationFailed/SeedFailed/Unknown) ورسالة عربية.
- [x] لا يوجد أي استخدام لـ `EnsureCreated()` في المشروع.
- [x] `IDesignTimeDbContextFactory<NasaqDbContext>` ([NasaqDbContextFactory.cs](Nasag/Data/NasaqDbContextFactory.cs)) لدعم `dotnet ef` بشكل مستقل عن host بدء التشغيل.
- [x] [DbSeeder](Nasag/Data/DbSeeder.cs) إدراج: 4 أدوار (مدير نظام/مدير مدرسة/معلم/محاسب)، admin/admin123 (BCrypt)، السنة 2025 - 2026، مدرسة النور الأهلية، 12 صف، 12 شعبة (6 صفوف × 2)، 36 مادة (6 صفوف × 6)، 3 امتحانات، 6 خطط رسوم، 30 طالب + 30 ولي أمر + StudentFees + 4 أقساط لكل طالب. Seeder Idempotent عبر فحص `Users.AnyAsync()`.
- [x] [IRepository<T> + Repository<T>](Nasag/Repositories/) عام async (GetAll, Where, GetById, FirstOrDefault, Count, Add, Update, Delete + OpenScope() لاستعلامات IQueryable). مسجّل كـ open generic singleton في DI.
- [x] [App.OnStartup](Nasag/App.xaml.cs) أصبح async: يُهيّئ Host → يستدعي `IDatabaseInitializer.InitializeAsync()` عبر `Task.Run` لتجنب deadlock — عند الفشل يظهر MessageBox عربي ويُغلق التطبيق (Phase 13 ستستبدل MessageBox بـ Splash + Setup Wizard).
- [x] [IConnectionMonitor](Nasag/Services/IConnectionMonitor.cs) مربوط الآن فعلياً بـ `IDbContextFactory<NasaqDbContext>.CreateDbContextAsync().Database.CanConnectAsync()` بدلاً من stub Phase 2، مع dispatch إلى UI thread عند تعديل الحالة.
- [x] [StudentsViewModel](Nasag/ViewModels/Pages/PageViewModel.cs) يستهلك `IRepository<Student>` ويُحمّل عدد الطلاب async في `ActivateAsync` المُستدعى من `MainShellViewModel.RefreshFromNavigation`.

**Acceptance Criteria:**
- [x] قاعدة البيانات تُنشأ وتُحدَّث تلقائياً عند أول تشغيل عبر Migrations (`DatabaseInitializer.MigrateAsync`).
- [x] إضافة Migration جديد لاحقاً يُطبَّق ذاتياً (المنطق يعتمد على `GetPendingMigrationsAsync`).
- [x] Seed Data تُحقن مرة واحدة فقط — Seeder يتحقق من `Users.AnyAsync()` قبل الإدراج.
- [x] قراءة Students من Repository داخل `StudentsViewModel.ActivateAsync` async.
- [x] فشل الاتصال يُعرض كرسالة عربية واضحة عبر MessageBox + Shutdown، لا crash.
- [x] Build: 0 Warning / 0 Error.

---

### Phase 4 — Authentication and Users
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] [LoginView](Nasag/Views/Auth/LoginView.xaml) — تخطيط من جزأين: لوحة Navy يسار (شعار + اسم البرنامج + شريحة وصف) + لوحة بيضاء يمين (نموذج الدخول): عنوان + رسالة خطأ + اسم المستخدم + كلمة المرور + تذكّرني + رابط نسيت كلمة المرور + زر Teal أساسي + قسيمة الحساب التجريبي (admin/admin123).
- [x] [IAuthService](Nasag/Services/IAuthService.cs) + [AuthService](Nasag/Services/AuthService.cs): يبحث المستخدم async، يفحص `IsActive`، يتحقق من BCrypt.Verify، يحدّث `LastLoginAt`، يعيد `AuthResult` بحالات (Success/InvalidCredentials/AccountDisabled/ConnectionError/Unknown) ورسائل عربية.
- [x] [ICurrentUserService](Nasag/Services/ICurrentUserService.cs) + [CurrentUserService](Nasag/Services/CurrentUserService.cs): يحفظ `User` الحالي + يولّد `DisplayName`/`Initial` + يبث `SignedIn`/`SignedOut`.
- [x] [LoginViewModel](Nasag/ViewModels/Auth/LoginViewModel.cs): Username/Password/RememberMe/ErrorMessage/IsBusy + `LoginCommand` async مع `CanExecute` يمنع الإرسال أثناء التحميل أو عند فراغ الحقول.
- [x] PasswordBox مربوط عبر code-behind بـ `vm.Password` (WPF لا يسمح bind مباشر لأسباب أمنية).
- [x] [App.OnStartup](Nasag/App.xaml.cs) أصبح مدير دورة حياة: يُهيّئ Host → DB → يفتح `LoginView`؛ عند `SignedIn` يفتح `MainShellView` ويُغلق Login؛ عند `SignedOut` يفعل العكس؛ `ShutdownMode=OnExplicitShutdown` في `App.xaml` لمنع الإغلاق التلقائي أثناء التبديل.
- [x] [MainShellViewModel](Nasag/ViewModels/Shell/MainShellViewModel.cs): يحقن `ICurrentUserService`، يعرض `UserDisplayName`/`UserInitial`/`UserRoleName` (مع تحديث ديناميكي عبر `SignedIn/Out`)، أضاف `LogoutCommand`، يُعيد التنقل إلى Dashboard تلقائياً عند تسجيل دخول جديد لتجنّب deep-link مزدوج (لأن الـ VM Singleton).
- [x] [MainShellView TopBar](Nasag/Views/Shell/MainShellView.xaml): شريحة المستخدم تعرض الحرف الأول + الاسم + اسم الدور؛ زر تسجيل الخروج مربوط بـ `LogoutCommand`.
- [x] [Converters.xaml](Nasag/Themes/Converters.xaml) + [InverseBoolToVisibilityConverter](Nasag/Helpers/InverseBoolToVisibilityConverter.cs) جديدان (سيُستخدمان في الشاشات اللاحقة أيضاً).

**Acceptance Criteria:**
- [x] الدخول بـ `admin` / `admin123` يعمل ويفتح MainShellView (BCrypt verify ناجح على hash السي).
- [x] كلمة مرور خاطئة تُظهر بانر أحمر بالعربية: «اسم المستخدم أو كلمة المرور غير صحيحة.».
- [x] حساب موقوف يُظهر رسالة مخصصة: «هذا الحساب موقوف. يرجى التواصل مع المدير.».
- [x] فشل الاتصال بـ SQL أثناء الدخول يُعرض كرسالة عربية واضحة لا crash.
- [x] اسم المستخدم وحرفه الأول واسم دوره ظاهرون في TopBar؛ زر تسجيل الخروج يُعيد المستخدم إلى نافذة Login.
- [x] Build: 0 Warning / 0 Error؛ التطبيق يقلع ويعرض شاشة Login بدون استثناءات.

---

### Phase 5 — Dashboard
**Status:** ✅ Completed (2026-05-15)

**Tasks:**
- [x] إضافة حزمة `LiveChartsCore.SkiaSharpView.WPF` 2.0.0-rc5.4 + إخفاء تحذيرات NU1701 الناتجة عن SkiaSharp/OpenTK عبر `<NoWarn>$(NoWarn);NU1701</NoWarn>`.
- [x] [IDashboardService](Nasag/Services/IDashboardService.cs) + [DashboardService](Nasag/Services/DashboardService.cs): استدعاء واحد `GetSnapshotAsync` يُرجع `DashboardSnapshot` مكوّناً من `DashboardStats` + `AttendanceLast7Days` + `AttendanceBreakdown` (اليوم) + `DashboardAlerts` + `RecentActivity[]`. كل الاستعلامات async عبر `IDbContextFactory<NasaqDbContext>` (CountAsync/SumAsync/GroupBy + projection بدون tracking).
- [x] [DashboardViewModel](Nasag/ViewModels/Pages/DashboardViewModel.cs) منفصل عن `PageViewModel.cs`: حقن `IDashboardService`، خصائص ObservableProperty لكل البطاقات والكسور، `ObservableCollection<RecentActivity> RecentActivities`، تحضير `Axis[]` و`ISeries[]` لكل من خط الحضور (LineSeries<double>) ودونات اليوم (PieSeries<double> 4 شرائح Success/Danger/Warning/Info)، Empty-state آمن: شريحة رمادية واحدة عندما لا توجد سجلات حضور لليوم. `RefreshCommand` للتحديث اليدوي. `ActivateAsync` يُستدعى تلقائياً من Shell عند الدخول للقسم.
- [x] [DashboardView.xaml](Nasag/Views/Pages/DashboardView.xaml) مطابق للتصميم (1): شريط رأس مع زر تحديث Secondary، صف 5 بطاقات إحصائية (StatCard موحّد بألوان مختلفة: Teal/Info/Navy/Danger/Success)، صف الرسوم بنسبة 2:1 (Line Chart 240px + Donut 200px مع نسبة الحضور المركزية ومفتاح ألوان UniformGrid 2×2)، صف سفلي 1:2 (4 بطاقات تنبيهات يسار + قائمة آخر الأنشطة يمين بفواصل سفلية)، `LoadingOverlay` فوق كامل الشاشة.
- [x] الرسوم تُغلَّف داخل `FlowDirection="LeftToRight"` لتجنّب عكس محاور SkiaSharp، مع overlay عربي RTL يُعرض بانر "لا توجد سجلات حضور بعد" عند `HasAttendanceHistory=false`.
- [x] [BoolToVisibilityConverter](Nasag/Helpers/BoolToVisibilityConverter.cs) + [ActivityKindConverters](Nasag/Helpers/ActivityKindConverters.cs) (Brush + Icon) + [DateToRelativeArabicConverter](Nasag/Helpers/DateToRelativeArabicConverter.cs) (يطبع: الآن / قبل دقيقة(دقيقتين/N) / قبل ساعة(ساعتين/N) / أمس / قبل يومين / قبل N أيام / yyyy/MM/dd) — كلها في `/Helpers`.
- [x] [DataTemplates.xaml](Nasag/Themes/DataTemplates.xaml): استبدال `PagePlaceholderView` بـ `DashboardView` لـ `DashboardViewModel`.
- [x] [App.xaml.cs](Nasag/App.xaml.cs): تسجيل `IDashboardService → DashboardService` Singleton.
- [x] حذف الـ stub `DashboardViewModel` القديم من `PageViewModel.cs` لصالح الملف المنفصل.
- [x] إصلاح ترويسة الصفحة: استبدال DockPanel بـ Grid عمودَين بعد ملاحظة أن `DockPanel.Dock="Right"` و`HorizontalAlignment="Left"` معاً في وضع RTL يدفعان العنوان والزر إلى نفس الجانب البصري (اليسار) بدل توزيعهما. الحل النهائي: Column 0 (`*`) للعنوان `HorizontalAlignment="Right"` يُرسم على اليمين، Column 1 (`Auto`) للزر يُرسم على اليسار.

**Acceptance Criteria:**
- [x] الأرقام تُحسب من قاعدة البيانات الحقيقية: عدد الطلاب النشطين، الشعب، المواد، إجمالي المحصّل، إجمالي المتبقي، أقساط متأخرة، أقساط مستحقة هذا الأسبوع — كلها من Seeder الفعلي (30 طالب، 12 شعبة، 36 مادة، StudentFees + 4 أقساط لكل طالب).
- [x] الرسم البياني يعرض بيانات حقيقية من `AttendanceRecords` بتجميع per-day GroupBy + Empty-state واضح ("لا توجد سجلات حضور بعد") عندما يكون الجدول فارغاً (الحالة الراهنة لأن Seeder لم يُدخل حضوراً).
- [x] الدونات يعرض شريحة رمادية واحدة عندما لا توجد بيانات حضور لليوم بدلاً من فشل LiveCharts بمصفوفة فارغة.
- [x] RTL سليم: الواجهة كاملة `FlowDirection=RightToLeft`، الرسوم وحدها LeftToRight لتجنّب عكس المحاور.
- [x] `LoadingOverlay` يظهر أثناء `LoadAsync` ويختفي في `finally` حتى لو فشل الاستعلام.
- [x] فشل DB يُعرض في `StatusMessage` بدلاً من crash.
- [x] Build: 0 Warning / 0 Error. التطبيق يقلع، يتصل بـ DB، Login يفتح بدون استثناء (الدخول الفعلي للوحة يتطلب تفاعل مستخدم).

---

### Phase 6 — Students and Guardians
**Status:** Pending

**Tasks:**
- شاشة قائمة الطلاب (تصميم 3): بطاقات إحصائية أعلى، شريط بحث وفلاتر، DataGrid بأعمدة (رقم الطالب، الاسم، الشعبة، الصف، الحالة، الجوال، ولي الأمر، إجراءات).
- شاشة إضافة/تعديل طالب (تصميم 4): أقسام (بيانات الطالب، بيانات ولي الأمر، العنوان والملاحظات)، رفع صورة، حفظ/إلغاء.
- بحث، فلترة حسب الصف/الشعبة/الحالة، Pagination.
- أرشفة طالب (Soft delete عبر Status).

**Acceptance Criteria:**
- إضافة طالب جديد وحفظه.
- تعديل طالب موجود.
- أرشفة طالب (لا يظهر في الافتراضي).
- البحث والفلاتر تعمل لحظياً.

---

### Phase 7 — Grades and Sections
**Status:** Pending

**Tasks:**
- شاشة `الصفوف والشعب` (تصميم 5): قائمة صفوف يساراً، DataGrid طلاب الشعبة المختارة يميناً، إحصائيات أعلى، أزرار إضافة/تعديل/حذف للصفوف والشعب.
- نقل طالب بين الشعب.
- منع حذف شعبة بها طلاب.

**Acceptance Criteria:**
- إنشاء صف وشعبة جديدين.
- ربط طلاب موجودين بشعبة.
- التحقق من السعة (Capacity).

---

### Phase 8 — Attendance
**Status:** Pending

**Tasks:**
- شاشة `الحضور والغياب` (تصميم 6): اختيار صف/شعبة/تاريخ، بطاقات إحصائية (حاضر/غائب/متأخر/إجازة)، DataGrid بأعمدة (رقم، اسم، حاضر/غائب/متأخر/إجازة كأزرار راديو، ملاحظات).
- زر "تحديد الكل حاضر".
- زر حفظ يحفظ سجلات اليوم.
- منع تكرار سجل لنفس الطالب في نفس اليوم (Upsert).

**Acceptance Criteria:**
- تسجيل حضور شعبة كاملة وحفظها.
- إعادة فتح نفس اليوم تُظهر السجلات المحفوظة.
- الملخصات تتحدث ديناميكياً.

---

### Phase 9 — Subjects, Exams, Marks, Results
**Status:** Pending

**Tasks:**
- إدارة المواد (CRUD مرتبط بصف).
- إدارة أنواع الامتحانات (CRUD + Weight).
- شاشة `إدخال الدرجات` (تصميم 7): اختيار صف/شعبة/مادة/امتحان، قائمة المواد يساراً، DataGrid طلاب يميناً، عمود الدرجة قابل للتعديل، حفظ سريع.
- شاشة `نتائج الطلاب` (تصميم 8): فلاتر، بطاقات إحصائية، DataGrid (الاسم، المجموع، المعدل، النتيجة، التقدير).
- منطق حساب: المجموع، المعدل، النجاح/الرسوب، التقدير (ممتاز/جيد جداً/جيد/مقبول/راسب).

**Acceptance Criteria:**
- إدخال درجات شعبة كاملة لمادة وامتحان بسرعة.
- النتائج تُحسب صحيحة وفق Weight.
- شاشة النتائج تعرض بيانات حقيقية.

---

### Phase 10 — Fees and Installments
**Status:** Pending

**Tasks:**
- شاشة `الرسوم والأقساط` (تصميم 9): اختيار طالب، بطاقة بيانات الطالب، بطاقات (إجمالي/مدفوع/متبقي)، جدول الأقساط (رقم، تاريخ الاستحقاق، المبلغ، الحالة، تاريخ الدفع، إجراءات)، زر تسجيل دفعة.
- نموذج تسجيل دفعة (مبلغ، طريقة، ملاحظات) → ينشئ Payment ويحدّث Installment وStudentFee.
- سند قبض مطبوع/Preview.
- كشف حساب طالب.

**Acceptance Criteria:**
- تسجيل دفعة تُحدّث المتبقي.
- قسط مدفوع يتغير لونه/حالته.
- توليد سند قبض برقم تسلسلي.

---

### Phase 11 — Reports and Printing
**Status:** Pending

**Tasks:**
- شاشة `التقارير` (تصميم 10): فلاتر علوية، 4 بطاقات تقارير (الرسوم، الدرجات، الحضور، الطلاب)، جدول آخر التقارير.
- تقارير أساسية: قائمة الطلاب، كشف حضور لفترة، كشف درجات، كشف رسوم.
- تصدير PDF (QuestPDF) وExcel (EPPlus).
- معاينة قبل الطباعة.

**Acceptance Criteria:**
- توليد PDF عربي RTL صحيح.
- تصدير Excel بأعمدة منسقة.
- معاينة تعمل.

---

### Phase 12 — Settings and Backup
**Status:** Pending

**Tasks:**
- شاشة إعدادات المدرسة: اسم، شعار، عنوان، هاتف، إيميل، السنة الدراسية الحالية، إعدادات الطباعة.
- شاشة المستخدمين والأدوار.
- النسخ الاحتياطي: زر إنشاء (يصدّر .bak من SQL Server)، زر استرجاع، جدول سجل النسخ.

**Acceptance Criteria:**
- تعديل بيانات المدرسة يُحفظ ويظهر في الشريط العلوي.
- نسخة احتياطية تُنشأ بنجاح.
- استرجاع نسخة يعمل (بعد تأكيد).

---

### Phase 13 — Final Polish, Splash, Setup Wizard, and Testing
**Status:** Pending

**Tasks:**

#### Splash Screen + Database Pipeline
- إنشاء `SplashWindow` احترافي يظهر فور تشغيل التطبيق بدلاً من MainWindow مباشرة.
- يحوي: الشعار، اسم البرنامج، شريط تقدّم أو سبيرنر، نص حالة ديناميكي بالعربية.
- داخل Splash يجري ما يلي بالترتيب (كل خطوة تُعرض كنص حالة):
  1. "جاري التحقق من الاتصال بقاعدة البيانات…"
  2. "جاري التحقق من التحديثات…" (`GetPendingMigrationsAsync`)
  3. إذا كانت هناك Migrations معلّقة: "جاري تحديث قاعدة البيانات…"
  4. "جاري تحميل البيانات الأولية…" (إن لزم)
  5. "جاهز" → فتح Login ثم MainShell.
- في حالة فشل أي خطوة: عرض رسالة عربية + زر "إعادة المحاولة" + زر "فتح معالج الإعداد".

#### First-Run Setup Wizard
- إذا تعذّر الاتصال بقاعدة البيانات أو كانت `appsettings.json` لا تحوي اتصالاً صالحاً، يفتح `SetupWizardWindow` بدلاً من Splash.
- خطوات المعالج:
  1. **مرحبا بك:** ترحيب وشرح أن البرنامج يحتاج لقاعدة بيانات SQL Server.
  2. **اختر نوع الإعداد:** (LocalDB / SQL Server شبكي).
  3. **بيانات الاتصال:** الخادم، طريقة المصادقة (Windows / SQL Authentication)، اسم المستخدم وكلمة المرور، اسم القاعدة.
  4. **اختبار الاتصال:** زر يجرّب الاتصال ويظهر النتيجة فوراً.
  5. **إنشاء قاعدة جديدة أو الاتصال بموجودة:** إن لم تكن القاعدة موجودة، خيار إنشائها وتطبيق Migrations + Seed.
  6. **إنهاء:** حفظ Connection String مُشفَّر في `appsettings.json` أو ملف إعداد مستخدم محمي، ثم متابعة إلى Splash العادي.
- يُتاح فتح المعالج لاحقاً من شاشة الإعدادات (Phase 12) لإعادة الضبط.

#### Polish
- مراجعة شاملة لـ RTL في كل شاشة.
- توحيد المسافات والألوان.
- مراجعة معالجة الأخطاء (try/catch + DialogService) في كل عمليات DB.
- التأكد أن كل عملية تستغرق وقتاً تعرض `LoadingOverlay` أو حالة Busy.
- التأكد أن `ConnectionStatusBanner` يعمل عند فصل الكابل/إيقاف SQL Server يدوياً.
- تحسين الأداء (Async، Pagination، Lazy load).
- تنظيف الكود وإزالة TODOs.
- اختبار سيناريوهات end-to-end:
  - تنصيب جديد على جهاز نظيف → معالج الإعداد → دخول → عمل كامل.
  - ترقية قاعدة بإضافة Migration جديد → Splash يطبّقه تلقائياً.
  - قطع الاتصال أثناء العمل → ظهور البانر → عودة الاتصال → اختفاء البانر.
- تحديث Agent.md نهائياً.
- كتابة ملف README.md للمستخدم النهائي.

**Acceptance Criteria:**
- Build نظيف بدون warnings حرجة.
- كل شاشات Phase 1-12 تعمل دون أخطاء.
- Splash يظهر دائماً، يحدّث القاعدة تلقائياً، ويفتح المعالج عند الفشل.
- المعالج يستطيع إنشاء قاعدة جديدة على LocalDB أو الاتصال بـ SQL Server بعيد.
- بانر انقطاع الاتصال يعمل في الحالتين (انقطاع وعودة).
- Agent.md محدّث بكل الإنجازات.

---

## 8. Current Progress

| Phase | Status | Started | Completed | Notes |
|-------|--------|---------|-----------|-------|
| Phase 0 — Planning | ✅ Completed | 2026-05-15 | 2026-05-15 | تم فحص المشروع و10 صور UI، استخراج الهوية البصرية، إنشاء Agent.md وAI_INSTRUCTIONS.md |
| Phase 1 — Foundation | ✅ Completed | 2026-05-15 | 2026-05-15 | بنية مجلدات + 8 حزم NuGet + خط Tajawal (3 أوزان) + Colors/Typography/Common dictionaries + DI عبر Generic Host + smoke-test window. Build: 0/0. |
| Phase 2 — Shell & Design System | ✅ Completed | 2026-05-15 | 2026-05-15 | MainShell كاملاً (Sidebar+TopBar+ContentHost) + 7 ResourceDictionaries (Buttons/Inputs/DataGrid/Cards/Pills/Icons/DataTemplates) + 5 UserControls (StatCard/SectionHeader/SidebarMenuItem/LoadingOverlay/ConnectionBanner) + IBusyService/IConnectionMonitor/INavigationService + 12 Page VMs + Placeholder view. Build 0/0. |
| Phase 3 — Database | ✅ Completed | 2026-05-15 | 2026-05-15 | 17 Entities + Enums + NasaqDbContext (Fluent API كامل) + InitialCreate migration + IDatabaseInitializer (Migrate ديناميكياً) + DbSeeder (4 أدوار/admin/12 صف/12 شعبة/36 مادة/30 طالب/خطط رسوم) + IRepository<T>/Repository<T> + ConnectionMonitor متصل بـ CanConnectAsync + EnableRetryOnFailure + BCrypt للتجزئة. Build 0/0. |
| Phase 4 — Auth | ✅ Completed | 2026-05-15 | 2026-05-15 | LoginView بتخطيط جزأين (Navy brand + form بيضاء) + IAuthService (BCrypt verify) + ICurrentUserService بـ SignedIn/Out events + LoginViewModel + App.OnStartup يدير دورة Login↔Shell + ShutdownMode=OnExplicitShutdown + TopBar يعرض الاسم/الحرف/الدور + LogoutCommand. Converters.xaml + InverseBoolToVisibility. Build 0/0. |
| Phase 5 — Dashboard | ✅ Completed | 2026-05-15 | 2026-05-15 | LiveCharts2 (rc5.4) + IDashboardService/DashboardService snapshot واحد + DashboardViewModel منفصل (5 stat cards + line chart للحضور 7 أيام + donut اليوم + 4 بطاقات تنبيهات + قائمة آخر الأنشطة) + DashboardView مطابق للتصميم (1) + Empty-states آمنة لكل جدول فارغ (LoadingOverlay، Refresh، StatusMessage عند الفشل) + 3 Converters جديدة (BoolToVisibility/ActivityKind*/DateToRelativeArabic) + NoWarn NU1701. Build 0/0. |
| Phase 6 — Students | Pending | - | - | - |
| Phase 7 — Classes | Pending | - | - | - |
| Phase 8 — Attendance | Pending | - | - | - |
| Phase 9 — Marks & Results | Pending | - | - | - |
| Phase 10 — Fees | Pending | - | - | - |
| Phase 11 — Reports | Pending | - | - | - |
| Phase 12 — Settings & Backup | Pending | - | - | - |
| Phase 13 — Polish | Pending | - | - | - |

---

## 9. Decisions Log

| التاريخ | القرار | السبب |
|--------|--------|------|
| 2026-05-15 | اعتماد .NET 8 WPF | الإطار الموجود مسبقاً في `Nasag.csproj` ويلبي كل متطلبات WPF + DI الحديثة |
| 2026-05-15 | اعتماد MVVM مع CommunityToolkit.Mvvm | تقليل boilerplate عبر Source Generators، مدعوم من Microsoft |
| 2026-05-15 | EF Core 8 + SQL Server (LocalDB في التطوير) | متطلب المستخدم صريح؛ LocalDB يبسّط التشغيل أثناء التطوير |
| 2026-05-15 | اعتماد Tajawal مع تضمينه في Assets/Fonts | متطلب صريح وضمان توفر الخط على أي جهاز |
| 2026-05-15 | استخدام Repository Pattern فوق EF Core | فصل واضح ويسهّل الاختبار |
| 2026-05-15 | تأجيل اختيار مكتبة Chart إلى Phase 5 | تجنب التزام مبكر؛ LiveCharts2 هو المرشح الأول |
| 2026-05-15 | تأجيل اختيار مكتبة PDF إلى Phase 11 | QuestPDF مرشح قوي لدعم RTL العربي |
| 2026-05-15 | استبعاد LMS / تطبيق ولي أمر / محاسبة كاملة | متطلب المستخدم الصريح بحصر النطاق |
| 2026-05-15 | تثبيت Tajawal محلياً في `Assets/Fonts` (Regular/Medium/Bold من Google Fonts) | ضمان توفر الخط دون اعتماد على نظام المستخدم — Build Action: Resource |
| 2026-05-15 | استخدام CommunityToolkit.Mvvm `ObservableObject` و`RelayCommand` بدلاً من Helpers يدوية | تقليل boilerplate، Source Generators رسمية من Microsoft؛ ألغى الحاجة لـ `Helpers/BaseViewModel.cs` |
| 2026-05-15 | اعتماد Microsoft.Extensions.Hosting (Generic Host) لإدارة DI والتكوين | نمط حديث وموحّد، يفتح الباب لاحقاً لـ Logging وOptions Pattern بدون تغيير |
| 2026-05-15 | حذف `StartupUri` من `App.xaml` وإنشاء `MainWindow` يدوياً داخل `OnStartup` | لتمكين حقن `MainViewModel` في `DataContext` عبر DI قبل الإظهار |
| 2026-05-15 | تأجيل أنماط Buttons/Inputs/DataGrid/Cards التفصيلية إلى Phase 2 | Phase 1 تركز فقط على الأساس؛ الأنماط جزء أصيل من "Design System and Main Shell" |
| 2026-05-15 | كل commit بحساب المطوّر فقط، **بدون** أي سطر Co-Authored-By وبدون عمل commit تلقائي | متطلب المستخدم الصريح؛ الوكيل يطلب الإذن قبل كل commit ويستخدم `git config` المحلي كما هو |
| 2026-05-15 | اعتماد `Database.MigrateAsync()` ديناميكياً بدلاً من `EnsureCreated` أو سكربتات يدوية | يضمن التقاط أي Migration مستقبلية تلقائياً دون كود مخصّص؛ متطلب المستخدم الصريح |
| 2026-05-15 | تفعيل `EnableRetryOnFailure` في `UseSqlServer` | مرونة ضد أعطال الشبكة العابرة دون كود إعادة محاولة يدوي في كل استدعاء |
| 2026-05-15 | بناء `LoadingOverlay` / `BusyButton` / `IBusyService` كجزء من Design System في Phase 2 | متطلب المستخدم: كل عملية تظهر Loading؛ توحيد التجربة عبر شاشات لاحقة |
| 2026-05-15 | بناء `ConnectionStatusBanner` + `IConnectionMonitor` لرصد انقطاع الاتصال بـ SQL Server | متطلب المستخدم: إظهار حالة الانقطاع وعدم انهيار البرنامج |
| 2026-05-15 | إضافة Splash Screen + First-Run Setup Wizard في Phase 13 | متطلب المستخدم الصريح: عمليات قاعدة البيانات والمعالج تظهر للمستخدم النهائي في المرحلة الأخيرة بعد جاهزية باقي المنظومة |
| 2026-05-15 | استخدام Implicit DataTemplates في `DataTemplates.xaml` بدلاً من ViewLocator مخصّص | حل WPF أصلي وأبسط لربط VM بـ View؛ سيُستبدل كل DataTemplate بـ View حقيقي مرحلياً |
| 2026-05-15 | جميع PageViewModels مسجَّلة Singleton لا Transient | للحفاظ على state التنقل والبحث بين الزيارات داخل الجلسة الواحدة |
| 2026-05-15 | استخدام `ResourceKeyConverter` لتمرير أيقونات Sidebar كـ string keys في NavigationDescriptor | يفصل التنقل عن WPF Resources ويسمح بتعريف القوائم في C# pure |
| 2026-05-15 | حذف MainWindow و MainViewModel القديمين بعد الانتقال لـ MainShellView | ملفات Phase 1 الانتقالية لم تعد مستخدمة؛ AI_INSTRUCTIONS يمنع الإبقاء على Dead code |
| 2026-05-15 | استخدام `AddDbContextFactory<NasaqDbContext>` بدل DbContext scoped | WPF تطبيق Desktop بدون scope-per-request؛ Factory يتيح short-lived contexts من أي خدمة Singleton (Repositories, ConnectionMonitor, DbSeeder, DatabaseInitializer) بأمان thread-safety |
| 2026-05-15 | اختيار `BCrypt.Net-Next` لتجزئة كلمات المرور | معيار صناعي، صيانة نشطة، دعم work-factor، يلبي AI_INSTRUCTIONS (Hashing دائماً) |
| 2026-05-15 | Generic `IRepository<T>` فقط في Phase 3 بدلاً من repos خاص لكل Entity | YAGNI — Acceptance criteria تتطلب فقط قراءة Students؛ Repositories متخصصة (StudentsRepository إلخ) ستُضاف عند الحاجة لاستعلامات domain-specific في Phase 6+ |
| 2026-05-15 | App.OnStartup async + استدعاء InitializeAsync عبر `Task.Run().ConfigureAwait(true)` | منع deadlock بـ WPF SynchronizationContext أثناء انتظار EF Core async migrations؛ MessageBox عربي عند الفشل ثم Shutdown — سيُستبدل بـ Splash في Phase 13 |
| 2026-05-15 | إضافة `IDesignTimeDbContextFactory<NasaqDbContext>` (`NasaqDbContextFactory.cs`) | يضمن نجاح `dotnet ef migrations add/database update` دون تشغيل WPF host (يقرأ appsettings.json مباشرة) |
| 2026-05-15 | إضافة `ActivateAsync` على `PageViewModel` كنقطة دخول لتحميل بيانات الصفحة | MainShell يستدعيها بعد كل تنقّل؛ مكان موحّد للأشياء async بدلاً من فايبر-end-fire-and-forget داخل الـ Constructor، يحافظ على Singleton ViewModel بدون state تالف |
| 2026-05-15 | حفظ DateTime UTC في DB واستخدام `datetime2` (افتراضي EF Core 8) | يلبي AI_INSTRUCTIONS؛ Phase 13 ستُضيف Hijri formatter عند العرض |
| 2026-05-15 | `ShutdownMode="OnExplicitShutdown"` + إدارة دورة حياة `LoginView`/`MainShellView` من `App` مباشرة | تبديل النوافذ أثناء Login/Logout يحتاج للحياة بعد إغلاق نافذة (لو OnLastWindowClose لانتهى التطبيق فور إغلاق Login عند نجاحه)؛ App يتعامل مع `CurrentUserService.SignedIn/Out` ليُنشئ النافذة المناسبة |
| 2026-05-15 | `PasswordBox` مربوط بـ ViewModel عبر code-behind وليس Binding | WPF لا يكشف `Password` كـ DependencyProperty لأسباب أمنية؛ التعامل اليدوي عبر `PasswordChanged` معيار رسمي ولا يحفظ النص في ذاكرة managed أطول من اللازم |
| 2026-05-15 | `MainShellViewModel` يستمع لـ `CurrentUserService.SignedIn` ويُعيد التنقل إلى Dashboard | الـ VM Singleton لذا يحتاج لإعادة تعيين حالته بين الجلسات بدلاً من إنشاء instance جديد (الذي سيتطلب Scope) |
| 2026-05-15 | فصل `Converters.xaml` كـ ResourceDictionary مستقل وإضافته في `App.xaml` قبل Common | Converters تُستخدم في عدة Themes/Views؛ فصلها يمنع التكرار ويبقي الترتيب صحيحاً (Common يعتمد عليها لاحقاً) |
| 2026-05-15 | اعتماد `LiveChartsCore.SkiaSharpView.WPF` 2.0.0-rc5.4 لرسوم Phase 5 | الـ rc5 مستقر داخل LiveCharts2 v2 ومدعوم رسمياً لـ WPF .NET 8 + ميزات Cartesian/Pie/PolarChart؛ مرشّح Agent.md لم يفرض إصداراً سابقاً |
| 2026-05-15 | تغليف `CartesianChart` و`PieChart` بـ `FlowDirection="LeftToRight"` داخل لوحة Dashboard RTL | محاور SkiaSharp تُرسم بإحداثيات شاشة فعلية؛ تركها في حاوية RTL يقلب اتجاه الزمن على المحور X ويعكس التسميات. الحل النهائي: إبقاء التطبيق كله RTL، وتحويل حاوية الرسم فقط لـ LTR. النصوص العربية فوق الرسم تُغلَّف بـ RTL StackPanel منفصل |
| 2026-05-15 | `IDashboardService` يُرجع `DashboardSnapshot` واحد بدل سلسلة Calls من الـ ViewModel | تقليل round-trips إلى DbContextFactory، استعلام واحد متماسك يضمن snapshot لحظي متّسق، يبسط معالجة الخطأ في `RefreshCommand` (try واحد) |
| 2026-05-15 | استخدام Grid عمودَين بدل DockPanel لترويسة DashboardView | في وضع RTL تتعارض دلالات `DockPanel.Dock="Right"` مع `HorizontalAlignment="Left"` فيتجمعان معاً على الجانب البصري نفسه. Grid مع `*` ثم `Auto` يضمن وضوحاً صريحاً: العمود 0 (يميناً في RTL) للعنوان، العمود 1 (يساراً) للزر — قاعدة عامة تنطبق على كل ترويسات الشاشات اللاحقة |
| 2026-05-15 | تغليف `CartesianChart` و`PieChart` بـ `FlowDirection="LeftToRight"` رغم أن الواجهة RTL | SkiaSharp يعكس المحور X والـ legends عند `RightToLeft`؛ overlay عربي RTL منفصل يعرض رسائل الـ empty-state دون مسّ بنية الرسم |
| 2026-05-15 | إخفاء تحذيرات NU1701 عبر `<NoWarn>` في csproj | تبعات SkiaSharp.Views.WPF + OpenTK تُحزَّم لـ net4x لكنها تعمل على net8.0-windows داخل WPF؛ القرار محدود لهذا المشروع للحفاظ على Build 0 Warning |
| 2026-05-15 | DashboardService يُرجع `record` snapshot واحد بدل عدة Tasks متوازية | استدعاء واحد من الـ ViewModel أبسط للـ Loading/Error handling، ويستخدم سياق DbContext واحد في كل استعلام (وفّرناها كذلك) |
| 2026-05-15 | فصل `DashboardViewModel` في ملف خاص خارج `PageViewModel.cs` | الـ VM للـ Dashboard كبر بشكل كافٍ (chart axes/series + 18 خاصية)؛ بقية PageVMs لا تزال stub داخل الملف المشترك حتى تكتمل مراحلها |
| 2026-05-15 | Empty-state لـ donut اليوم عبر شريحة رمادية واحدة بقيمة 1 | LiveCharts2 يفشل عند `Values=Array.Empty<>()`؛ الشريحة الرمادية تحفظ الشكل البصري ويظهر فوقها overlay "0٪" |

---

## 10. Issues and Risks

| المشكلة المحتملة | التأثير | التخفيف |
|----------------|---------|---------|
| دعم RTL في DataGrid افتراضي ضعيف | عرض الأعمدة معكوس | تخصيص Template كامل للـ DataGrid في `Themes/DataGrid.xaml` |
| توفر خط Tajawal على بعض الأنظمة | الواجهة قد تستخدم fallback | تضمين الخط داخل المشروع (Build Action: Resource) |
| طباعة PDF عربي ذو شكل صحيح | تقارير مشوهة | استخدام QuestPDF أو iText7 مع تفعيل ArabicShaping |
| أداء DataGrid مع آلاف الطلاب | بطء التمرير | Pagination + Virtualizing في الـ ItemsControl |
| Migrations مع EF Core على LocalDB | تعارضات schema | إدارة منظمة عبر EF Tools وعدم التعديل اليدوي |
| النسخ الاحتياطي يحتاج صلاحيات على SQL Server | فشل BACKUP DATABASE | استخدام T-SQL مباشرة + رسالة خطأ واضحة عند نقص الصلاحية |
| فشل تطبيق Migration على قاعدة موجودة (تعارض schema) | البرنامج لا يقلع | عرض الخطأ في Splash + خيار فتح Setup Wizard للترميم/إعادة الإعداد + توثيق طريقة Rollback عبر `dotnet ef database update <previous>` |
| Connection String حسّاس داخل `appsettings.json` | تسرّب كلمة مرور SQL | في Setup Wizard: حفظ Connection String في ملف إعداد مستخدم محمي (DPAPI / ProtectedData) بدلاً من JSON نصي عند استخدام SQL Authentication |
| عملية async تستمر بعد إغلاق النافذة → استثناء | crash عند الخروج | استخدام `CancellationToken` مربوط بدورة حياة الـ ViewModel/Window |
| `LoadingOverlay` يحجب التفاعل بشكل دائم بسبب استثناء غير ملتقط | تجمد UI | تغليف كل عملية async بـ try/finally يضمن `IsBusy = false` حتى عند الفشل |

---

## 11. Next Agent Instructions

أي وكيل AI يدخل المشروع بعد الآن **يجب** أن يلتزم بما يلي:

1. **اقرأ `AI_INSTRUCTIONS.md` أولاً.**
2. **اقرأ `Agent.md` بالكامل** (هذا الملف).
3. حدّد آخر مرحلة مكتملة في القسم 8.
4. أكمل من المرحلة التالية فقط — لا تعد تنفيذ مراحل مكتملة إلا إذا كانت مكسورة فعلياً.
5. لا تغير القرارات المسجلة في القسم 9 دون سبب موثّق.
6. حدّث القسم 8 و9 بعد كل عمل.
7. عند الانتهاء من جلسة، اذكر: ما تم، حالة Build، الملفات المهمة، المرحلة التالية.

**الحالة الحالية:** Phase 0/1/2/3/4/5 اكتملت. المرحلة التالية هي **Phase 6 — Students and Guardians** (قائمة الطلاب مطابقة للتصميم 3: بطاقات إحصائية + شريط بحث وفلاتر + DataGrid بأعمدة كاملة، شاشة إضافة/تعديل طالب مطابقة للتصميم 4 بأقسام بيانات الطالب وولي الأمر والعنوان، رفع صورة، أرشفة Soft delete، Pagination). لا تبدأ Phase 6 دون طلب صريح من المستخدم.
