package main

import (
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

func main() {
	fs := http.StripPrefix("/download/", http.FileServer(http.Dir("./file")))
	http.Handle("/", fs)
	http.HandleFunc("/upload", uploadHandler)
	//http.HandleFunc("/download/", downloadHandler)
	http.ListenAndServe(":8080", nil)
}

func uploadHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method == "GET" {
		http.ServeFile(w, r, r.URL.Path[1:])
		return
	}

	if r.Method == "POST" {
		err := r.ParseMultipartForm(32 << 20)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		folder, err := os.Getwd()
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}

		files := r.MultipartForm.File["file"]
		for _, file := range files {
			src, err := file.Open()
			if err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			defer src.Close()

			dst, err := os.Create(filepath.Join(folder, file.Filename))
			if err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			defer dst.Close()

			if _, err = io.Copy(dst, src); err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}

			fmt.Fprintf(w, "Successfully uploaded file %s\n", file.Filename)
		}
	}
}

func downloadHandler(w http.ResponseWriter, r *http.Request) {
	filename := strings.TrimLeft(r.URL.Path, "/download")
	filePath := filepath.Join(".", "file", filename)
	f, err := os.Open(filePath)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	defer f.Close()
	fileInfo, err := f.Stat()
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	// 设置Content-Disposition字段
	w.Header().Set("Content-Disposition", "attachment; filename="+fileInfo.Name())
	w.Header().Set("Content-Type", "application/octet-stream")
	w.Header().Set("Content-Length", fmt.Sprintf("%d", fileInfo.Size()))

	_, err = io.Copy(w, f)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
}
