using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Boxes.Models;

namespace Boxes.Services;

public class FileScanner
{
    public async Task<List<FileItem>> ScanDesktopAsync()
    {
        return await Task.Run(() =>
        {
            var files = new List<FileItem>();
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            try
            {
                var fileInfos = new DirectoryInfo(desktopPath)
                    .GetFiles()
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .ToList();
                
                foreach (var fileInfo in fileInfos)
                {
                    try
                    {
                        var fileItem = new FileItem
                        {
                            FullPath = fileInfo.FullName,
                            Name = fileInfo.Name,
                            Extension = fileInfo.Extension.ToLower(),
                            SizeInBytes = fileInfo.Length,
                            CreatedDate = fileInfo.CreationTime,
                            ModifiedDate = fileInfo.LastWriteTime,
                            FileType = DetermineFileType(fileInfo.Extension),
                            IconGlyph = GetIconForFileType(fileInfo.Extension)
                        };
                        
                        files.Add(fileItem);
                    }
                    catch
                    {
                        // Skip files that can't be accessed
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle directory access errors
                throw new Exception($"Failed to scan desktop: {ex.Message}", ex);
            }
            
            return files.OrderByDescending(f => f.ModifiedDate).ToList();
        });
    }
    
    public async Task<List<FileItem>> ScanDirectoryAsync(string path)
    {
        return await Task.Run(() =>
        {
            var files = new List<FileItem>();
            
            try
            {
                var fileInfos = new DirectoryInfo(path)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .ToList();
                
                foreach (var fileInfo in fileInfos)
                {
                    try
                    {
                        var fileItem = new FileItem
                        {
                            FullPath = fileInfo.FullName,
                            Name = fileInfo.Name,
                            Extension = fileInfo.Extension.ToLower(),
                            SizeInBytes = fileInfo.Length,
                            CreatedDate = fileInfo.CreationTime,
                            ModifiedDate = fileInfo.LastWriteTime,
                            FileType = DetermineFileType(fileInfo.Extension),
                            IconGlyph = GetIconForFileType(fileInfo.Extension)
                        };
                        
                        files.Add(fileItem);
                    }
                    catch
                    {
                        // Skip files that can't be accessed
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to scan directory: {ex.Message}", ex);
            }
            
            return files.OrderByDescending(f => f.ModifiedDate).ToList();
        });
    }
    
    private static string DetermineFileType(string extension)
    {
        return extension.ToLower() switch
        {
            ".doc" or ".docx" or ".txt" or ".rtf" or ".pdf" => "Document",
            ".xls" or ".xlsx" or ".csv" => "Spreadsheet",
            ".ppt" or ".pptx" => "Presentation",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".svg" or ".webp" => "Image",
            ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" => "Video",
            ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".m4a" => "Audio",
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "Archive",
            ".exe" or ".msi" or ".dmg" => "Application",
            ".html" or ".htm" or ".css" or ".js" or ".json" or ".xml" => "Code",
            ".cs" or ".py" or ".java" or ".cpp" or ".c" or ".h" => "Source Code",
            _ => "File"
        };
    }
    
    private static string GetIconForFileType(string extension)
    {
        return extension.ToLower() switch
        {
            ".doc" or ".docx" or ".txt" or ".rtf" or ".pdf" => "\uE8A5", // Document
            ".xls" or ".xlsx" or ".csv" => "\uE9F9", // DataSheet
            ".ppt" or ".pptx" => "\uE7C3", // Chart
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".svg" or ".webp" => "\uEB9F", // Photo
            ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" => "\uE8B2", // Video
            ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".m4a" => "\uE8D6", // MusicNote
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "\uE8B5", // ZipFolder
            ".exe" or ".msi" or ".dmg" => "\uE756", // App
            ".html" or ".htm" or ".css" or ".js" or ".json" or ".xml" or ".cs" or ".py" => "\uE943", // Code
            _ => "\uE8A5" // Generic file
        };
    }
}
