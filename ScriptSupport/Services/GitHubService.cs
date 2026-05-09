using System.IO;
using System.Diagnostics;
using LibGit2Sharp;
using GitCommands = LibGit2Sharp.Commands;

namespace ScriptSupport.Services
{
    public class GitHubService
    {
        public static async Task<(bool, string)> CheckForUpdatesAsync(string FolderPath, string GitHubUrl)
        {
            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            // Kiểm tra xem folder data có rỗng không
            bool isDataFolderEmpty = !Directory.EnumerateFileSystemEntries(FolderPath).Any();
            // Kiểm tra xem folder data có chứa Git repository không
            bool isGitRepository = Directory.Exists(Path.Combine(FolderPath, ".git"));

            try
            {
                if (isDataFolderEmpty)
                {
                    // Nếu folder rỗng, thực hiện clone
                    await Task.Run(() => DownLoadRepository(FolderPath, GitHubUrl));
                    return (true, string.Empty);
                }
                else
                {
                    if (isGitRepository)
                    {
                        // Nếu đã có Git repository, thử cập nhật
                        try
                        {
                            return await Task.Run(() => FetchAndResetToLatest(FolderPath, GitHubUrl));
                        }
                        catch (Exception ex) when (ex.Message.Contains("object not found") ||
                                                  ex.Message.Contains("no match for id"))
                        {
                            // Nếu gặp lỗi "object not found", thực hiện clone lại
                            CleanDirectory(FolderPath);
                            var (result, message) = await Task.Run(() => DownLoadRepository(FolderPath, GitHubUrl));
                            return (result, message);
                        }
                        catch (Exception ex)
                        {
                            return (false, ex.Message);
                        }
                    }
                    else
                    {
                        CleanDirectory(FolderPath);
                        var (result, message) = await Task.Run(() => DownLoadRepository(FolderPath, GitHubUrl));
                        return (result, message);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static (bool, string) DownLoadRepository(string FolderPath, string GitHubUrl)
        {
            try
            {
                // Xóa dữ liệu cũ nếu có
                if (!CleanDirectory(FolderPath))
                {
                    throw new Exception("Cannot Delete Folder");
                }

                // Thiết lập tùy chọn cho việc clone
                var options = new CloneOptions
                {
                    Checkout = true,
                    RecurseSubmodules = false
                };

                // Cài đặt thuộc tính TagFetchMode
                try
                {
                    var fetchOptions = new FetchOptions();
                    if (fetchOptions.GetType().GetProperty("TagFetchMode") != null)
                    {
                        fetchOptions.TagFetchMode = TagFetchMode.All;
                    }
                }
                catch
                {
                    ///
                }

                // Clone repository với tùy chọn đã thiết lập
                Repository.Clone(GitHubUrl, FolderPath, options);

                // Cấu hình repository để lấy tất cả các nhánh
                using (var repo = new Repository(FolderPath))
                {
                    // Lấy tất cả các nhánh từ remote
                    var remote = repo.Network.Remotes["origin"];
                    if (remote != null)
                    {
                        var refSpecs = new string[] { "+refs/heads/*:refs/remotes/origin/*" };
                        GitCommands.Fetch(repo, remote.Name, refSpecs, null, "Fetching all branches");

                        var defaultBranch = repo.Branches["master"] ?? repo.Branches["main"];
                        if (defaultBranch != null)
                        {
                            GitCommands.Checkout(repo, defaultBranch);
                        }
                    }
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static (bool, string) FetchAndResetToLatest(string FolderPath, string GitHubUrl)
        {
            bool hasChanges = false;

            try
            {
                using (var repo = new Repository(FolderPath))
                {
                    // Kiểm tra và sửa chữa remote nếu cần
                    var remote = repo.Network.Remotes["origin"];
                    if (remote == null)
                    {
                        // Thêm remote nếu không tồn tại
                        remote = repo.Network.Remotes.Add("origin", GitHubUrl);
                    }
                    else if (remote.Url != GitHubUrl)
                    {
                        // Cập nhật URL nếu đã thay đổi
                        repo.Network.Remotes.Update("origin", r => r.Url = GitHubUrl);
                    }

                    // Thiết lập fetch options
                    var fetchOptions = new FetchOptions
                    {
                        Prune = false
                    };

                    // Cài đặt TagFetchMode nếu có trong phiên bản này
                    if (fetchOptions.GetType().GetProperty("TagFetchMode") != null)
                    {
                        fetchOptions.TagFetchMode = TagFetchMode.All;
                    }

                    // Đảm bảo chúng ta lấy tất cả các nhánh
                    var refSpecs = new string[] { "+refs/heads/*:refs/remotes/origin/*" };

                    // Thực hiện fetch từ remote
                    GitCommands.Fetch(repo, remote.Name, refSpecs, fetchOptions, "Fetching updates");

                    // Lấy nhánh hiện tại
                    var currentBranch = repo.Head.FriendlyName;

                    // Lấy remote tracking branch tương ứng
                    var trackingBranch = repo.Branches[$"origin/{currentBranch}"];

                    if (trackingBranch != null && trackingBranch.Tip != null)
                    {
                        // Lấy commit ID của HEAD hiện tại
                        var currentCommitId = repo.Head.Tip.Id;

                        // Lấy commit ID của remote tracking branch
                        var remoteCommitId = trackingBranch.Tip.Id;

                        // Kiểm tra xem có cập nhật không
                        hasChanges = currentCommitId != remoteCommitId;

                        if (hasChanges)
                        {
                            try
                            {
                                // Hard reset về commit mới nhất của nhánh remote
                                repo.Reset(ResetMode.Hard, trackingBranch.Tip);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Lỗi khi reset: {ex.Message}");
                                throw;
                            }
                        }
                    }
                    else
                    {
                        // Trường hợp không tìm thấy nhánh tương ứng trên remote
                        var defaultRemoteBranch = repo.Branches["origin/master"] ?? repo.Branches["origin/main"];
                        if (defaultRemoteBranch != null)
                        {
                            GitCommands.Checkout(repo, defaultRemoteBranch.FriendlyName.Replace("origin/", ""));
                            hasChanges = true;
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy nhánh mặc định trên remote");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
                throw;
            }
            return (hasChanges, string.Empty);
        }

        private static bool CleanDirectory(string directoryPath)
        {
            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            try
            {
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Không thể xóa file {file.FullName}: {ex.Message}");
                    }
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Attributes = FileAttributes.Normal;
                        dir.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Không thể xóa thư mục {dir.FullName}: {ex.Message}");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

