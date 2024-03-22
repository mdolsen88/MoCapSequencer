close all
clear
clc

fid = fopen('./bin/Debug/Test.bin');
Data = [];
while ~feof(fid)
    ts = fread(fid,1,'int64');
    n = fread(fid,1,'int32');
    for i = 1:n
        id = fread(fid,1,'int32');
        xyz = fread(fid,3,'single');
        if isempty(Data)
            Data = cell(n,1);
        end
        Data{i}(:,end+1) = xyz;
    end
end
fclose(fid);
L = 8;
R = 12;

for i = 1:3
    d = Data{L}(i,:);
    subplot(2,3,i)
    plot(d,'-');
    subplot(2,3,i+3)
    plot(xcorr(d,30*10,'coeff'))
end