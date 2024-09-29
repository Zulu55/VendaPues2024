window.BlazorDownloadFile = (filename, content) => {
    const link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};