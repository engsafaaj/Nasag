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
**Status:** Pending

**Tasks:**
- بناء `MainShellView` يحتوي: Sidebar يميناً + TopBar أعلى + ContentHost في المنتصف.
- تنفيذ `Sidebar` بخلفية Navy، الشعار في الأعلى، قائمة عناصر تنقل (الرئيسية، الطلاب، الصفوف والشعب، الحضور، المواد والدرجات، النتائج، الرسوم، التقارير، المستخدمون، الإعدادات، النسخ الاحتياطي).
- تنفيذ `TopBar`: بحث، اختيار مدرسة، اختيار سنة، إشعارات، صورة + اسم المستخدم.
- إنشاء أنماط: `PrimaryButton`, `SecondaryButton`, `IconButton`, `DangerButton`.
- إنشاء أنماط الحقول: TextBox, PasswordBox, ComboBox, DatePicker بحواف مدورة.
- إنشاء نمط DataGrid عام.
- إنشاء UserControl `StatCard` و`StatusPill` و`SectionHeader`.
- **إنشاء UserControl `LoadingOverlay`** قابل للوضع فوق أي منطقة محتوى (شبه شفاف + سبيرنر دائري Teal + نص "جاري التحميل…" قابل للتخصيص) — يُربط بـ `IsBusy` على ViewModel.
- **إنشاء UserControl `ConnectionStatusBanner`** يظهر أعلى المحتوى عند فقدان الاتصال بقاعدة البيانات، بلون Danger مع زر "إعادة المحاولة".
- **إنشاء `BusyButton` (نمط Button موسّع)** يعطّل نفسه ويعرض سبيرنر داخلي عندما `IsBusy=True`.
- **إنشاء `IBusyService` / `BusyService`** لتنسيق حالة Busy عند تنفيذ عمليات async من الـ ViewModels.
- **إنشاء `IConnectionMonitor`** ينشر أحداث `Disconnected`/`Reconnected` ويستمع له ShellView لإظهار/إخفاء البانر.
- إعداد `NavigationService` و`ViewLocator` بسيط.
- شاشات وهمية فارغة لكل قسم.

**Acceptance Criteria:**
- الشكل العام مطابق للهوية البصرية (Navy sidebar + Teal accents + بطاقات بيضاء).
- التنقل بين الأقسام يعمل عبر القائمة الجانبية.
- لا توجد صور UI مستخدمة كخلفية.
- الأنماط مركزية في `/Themes` بدون تكرار.
- `LoadingOverlay` يمكن إظهاره على شاشة وهمية وربطه بـ ViewModel.IsBusy.
- `ConnectionStatusBanner` يظهر/يختفي ديناميكياً عند تغيّر حالة الاتصال.

---

### Phase 3 — Database and Core Entities
**Status:** Pending

**Tasks:**
- إنشاء كل Model في `/Models`.
- إنشاء `NasaqDbContext` مع DbSets والعلاقات (Fluent API).
- ضبط connection string في `appsettings.json` + قراءته عبر `IConfiguration` عند تسجيل `DbContext` في DI.
- تفعيل `EnableRetryOnFailure` في إعداد `UseSqlServer` لمعالجة أعطال شبكية عابرة.
- إنشاء أول Migration وتشغيلها على LocalDB.
- **بناء `IDatabaseInitializer` / `DatabaseInitializer` يطبّق Migrations ديناميكياً عند بدء التطبيق:**
  - `CanConnectAsync()` للتحقق من إمكانية الاتصال (مع تقرير الفشل بشكل واضح).
  - `GetPendingMigrationsAsync()` لجلب أي Migrations معلّقة (مستقبلية بما في ذلك).
  - `MigrateAsync()` لتطبيقها تلقائياً — أي Migration جديدة يضيفها أي وكيل لاحقاً تُلتقط دون كود إضافي.
  - استدعاء `IDbSeeder.SeedIfEmptyAsync()` بعد الـ Migrations عند فراغ القاعدة.
  - معالجة أخطاء (SqlException، Authentication، Network) وإرجاع نتيجة منظَّمة `DatabaseInitResult` (Success / Failed + Reason).
- **ممنوع استخدام `EnsureCreated()`** — يجب الاعتماد على Migrations فقط.
- كتابة `DbSeeder` لإدخال بيانات تجريبية (مدرسة النور، السنة 2025-2026، صفوف، شعب، مستخدم admin/admin123، ~30 طالب، مواد، رسوم).
- إنشاء واجهات Repositories لكل Entity رئيسي + التنفيذ، كلها async (Task-based).
- تسجيل الكل في DI.
- في `App.OnStartup`: استدعاء `DatabaseInitializer` قبل إظهار MainWindow (في Phase 3 يكفي عرض MessageBox عند الفشل؛ Phase 13 ستلفّ هذا بـ Splash + Wizard).

**Acceptance Criteria:**
- قاعدة البيانات تُنشأ وتُحدَّث تلقائياً عند أول تشغيل عبر Migrations.
- إضافة Migration جديد لاحقاً يُطبَّق ذاتياً دون أي تعديل كود إضافي.
- Seed Data تُحقن مرة واحدة فقط (يتحقق Seeder بأن القاعدة فارغة قبل الإدراج).
- يمكن قراءة Students من Repository داخل ViewModel بشكل async.
- فشل الاتصال يُعرض كرسالة عربية واضحة، لا crash.

---

### Phase 4 — Authentication and Users
**Status:** Pending

**Tasks:**
- شاشة `LoginView` مطابقة للتصميم رقم (2): بطاقة مركزية، شعار، حقلي مستخدم وكلمة مرور، خيار تذكرني، زر تسجيل دخول.
- `AuthService` يتحقق من Hash كلمة المرور (BCrypt أو PBKDF2).
- `CurrentUserService` يحتفظ بالمستخدم الحالي في الذاكرة.
- بعد الدخول الناجح: استبدال نافذة Login بـ MainShellView.
- ظهور اسم المستخدم وصورته في TopBar.
- زر تسجيل خروج.

**Acceptance Criteria:**
- الدخول بـ admin / admin123 يعمل.
- كلمة مرور خاطئة تُظهر خطأ.
- المستخدم يظهر في الشريط العلوي.

---

### Phase 5 — Dashboard
**Status:** Pending

**Tasks:**
- شاشة `DashboardView` مطابقة للتصميم (1): 5 بطاقات إحصائية + رسم بياني خطي للحضور + دونات للنسبة + بطاقات تنبيهات + جدول آخر الأنشطة.
- ربط البيانات بـ DashboardService يجمع الأرقام من Repositories.
- استخدام مكتبة رسوم بيانية (LiveCharts2).

**Acceptance Criteria:**
- الأرقام تُحسب من قاعدة البيانات الحقيقية (Seed).
- الرسم البياني يعرض بيانات.
- RTL سليم.

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
| Phase 2 — Shell & Design System | Pending | - | - | - |
| Phase 3 — Database | Pending | - | - | - |
| Phase 4 — Auth | Pending | - | - | - |
| Phase 5 — Dashboard | Pending | - | - | - |
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

**الحالة الحالية:** Phase 0 و Phase 1 اكتملتا. المرحلة التالية هي **Phase 2 — Design System and Main Shell** (بناء MainShell + Sidebar + TopBar + أنماط Buttons/Inputs/DataGrid/Cards + شاشات وهمية لكل قسم + NavigationService). لا تبدأ Phase 2 دون طلب صريح من المستخدم.
