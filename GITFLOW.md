# GitFlow - Quy Trình Làm Việc Cơ Bản

## Tổng Quan
GitFlow là một quy trình làm việc với Git được thiết kế để quản lý dự án phát triển phần mềm một cách có tổ chức và hiệu quả.

## Các Branch Chính

### 1. **main/master** (Branch Sản Phẩm)
- Chứa code đã được release và deploy lên production
- Chỉ được merge từ `develop` hoặc `hotfix` branches
- Mỗi commit trên branch này đại diện cho một version mới

### 2. **develop** (Branch Phát Triển)
- Branch chính để tích hợp các tính năng mới
- Chứa code đã được test và sẵn sàng cho release
- Được merge từ `feature` branches
- Khi đủ tính năng, sẽ được merge vào `main` để tạo release

### 3. **feature/** (Branch Tính Năng)
- Tạo từ `develop` để phát triển tính năng mới
- Naming convention: `feature/tên-tính-năng`
- Khi hoàn thành, merge lại vào `develop`
- Xóa sau khi merge

### 4. **release/** (Branch Release)
- Tạo từ `develop` khi chuẩn bị release
- Naming convention: `release/version-number` (VD: `release/1.0.0`)
- Chỉ sửa lỗi nhỏ và chuẩn bị metadata cho release
- Khi hoàn thành, merge vào cả `main` và `develop`
- Xóa sau khi merge

### 5. **hotfix/** (Branch Sửa Lỗi Khẩn Cấp)
- Tạo từ `main` để sửa lỗi nghiêm trọng trên production
- Naming convention: `hotfix/version-number` (VD: `hotfix/1.0.1`)
- Khi hoàn thành, merge vào cả `main` và `develop`
- Xóa sau khi merge

## Quy Trình Làm Việc

### 1. Bắt Đầu Tính Năng Mới
```bash
git checkout develop
git pull origin develop
git checkout -b feature/tên-tính-năng
```

### 2. Hoàn Thành Tính Năng
```bash
git add .
git commit -m "feat: mô tả tính năng"
git push origin feature/tên-tính-năng
# Tạo Pull Request từ feature -> develop
```

### 3. Chuẩn Bị Release
```bash
git checkout develop
git pull origin develop
git checkout -b release/1.0.0
# Sửa version, changelog, etc.
git commit -m "chore: prepare release 1.0.0"
git push origin release/1.0.0
```

### 4. Hoàn Thành Release
```bash
git checkout main
git merge release/1.0.0
git tag -a v1.0.0 -m "Release version 1.0.0"
git checkout develop
git merge release/1.0.0
git push origin main
git push origin develop
git push origin --tags
git branch -d release/1.0.0
```

### 5. Sửa Lỗi Khẩn Cấp
```bash
git checkout main
git pull origin main
git checkout -b hotfix/1.0.1
# Sửa lỗi
git commit -m "fix: mô tả lỗi đã sửa"
git checkout main
git merge hotfix/1.0.1
git tag -a v1.0.1 -m "Hotfix version 1.0.1"
git checkout develop
git merge hotfix/1.0.1
git push origin main
git push origin develop
git push origin --tags
git branch -d hotfix/1.0.1
```

## Quy Tắc Commit Message

Sử dụng format: `type: description`

- **feat**: Tính năng mới
- **fix**: Sửa lỗi
- **docs**: Cập nhật tài liệu
- **style**: Thay đổi format code (không ảnh hưởng logic)
- **refactor**: Refactor code
- **test**: Thêm hoặc sửa test
- **chore**: Cập nhật build process, dependencies, etc.

## Lưu Ý Quan Trọng

1. **Không bao giờ commit trực tiếp vào `main, develop`**
2. **Luôn pull latest code trước khi tạo branch mới**
3. **Xóa branch sau khi merge để giữ repository sạch sẽ**
4. **Sử dụng Pull Request để review code**
5. **Tag mỗi version release trên `main`**
6. **Merge `develop` vào `main` chỉ khi release**

## Lợi Ích Của GitFlow

- **Tổ chức**: Code được tổ chức rõ ràng theo mục đích
- **Stability**: `main` luôn chứa code ổn định
- **Collaboration**: Nhiều developer có thể làm việc song song
- **Release Management**: Quản lý version và release dễ dàng
- **Hotfix**: Xử lý lỗi production nhanh chóng
- **History**: Lịch sử thay đổi rõ ràng và có ý nghĩa
