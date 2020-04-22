import Axios, { AxiosPromise } from 'axios';
import CONFIG from '@/environment/app.config';

export default class AppService {
    getLine(offset: number){
        return Axios.get(`${CONFIG.API_URL}/app/line?offset=${offset}`);
    }

    getFile(): AxiosPromise<any>{
        return Axios.get(`${CONFIG.API_URL}/app/browse`);
    }

    updateText(index: number, text: string, autoReplace: boolean = false){
        return Axios.put(`${CONFIG.API_URL}/update/text?index=${index}&autoReplace=${autoReplace?1:0}`, text);
    }

    regex(pattern: string, replaceTo: string, regex: boolean=false, ignoreSensitive: boolean= false){
        return Axios.post(`${CONFIG.API_URL}/regex`,{
            pattern,
            text: replaceTo,
            regex: regex?1:0,
            ignoreSensitive: ignoreSensitive?1:0
        });
    }

    find(pattern: string, currentIndex: number, regex: boolean = false, ignoreSensitive: boolean =false, exact: boolean = false, direction: 'up' | 'down' = 'down'){
        return Axios.post(`${CONFIG.API_URL}/findIndex`,{
            pattern,
            currentIndex,
            exact: exact ? 1: 0,
            ignoreSensitive: ignoreSensitive?1:0,
            regex: regex? 1:0,
            direction
        });
    }

    writeToFile(filePath: string){
        return Axios.post(`${CONFIG.API_URL}/app/save`, filePath);
    }

    closeApp(){
        return Axios.post(`${CONFIG.API_URL}/app/close`);
    }
    undo(){
        return Axios.get(`${CONFIG.API_URL}/app/undo`);
    }
    redo(){
        return Axios.get(`${CONFIG.API_URL}/app/redo`);
    }

    extractFile(source: string, dest: string, encrypt: boolean=false){
        return Axios.post(`${CONFIG.API_URL}/app/run-script`, {
            source,
            dest,
            encrypt: encrypt?1:0
        });
    }
    patchFile(source: string, dest: string){
        return Axios.post(`${CONFIG.API_URL}/app/run-patcher`, {
            source,
            dest
        });
    }
    gzip(source: string, dest: string){
        return Axios.post(`${CONFIG.API_URL}/app/run-gzip`, {
            source,
            dest
        });
    }


    openExplorer(filePath: string){
        return Axios.post(`${CONFIG.API_URL}/app/explorer`, filePath);
    }

    fileDialog(txt: boolean =true, loc: boolean = true){
        let type=""
        if (txt && loc)
            type="txt,loc";
        else {
            if (loc)
                type="loc";
            else
                type="txt";
        }
        return Axios.get(`${CONFIG.API_URL}/app/file-dialog?type=${type}`);
    }

    openFileDialog(filter: string){
        return Axios.get(`${CONFIG.API_URL}/app/file-dialog?filter=${encodeURI(filter)}`);
    }
    saveFileDialog(filter: string){
        return Axios.get(`${CONFIG.API_URL}/app/file-save-dialog?filter=${encodeURI(filter)}`);
    }

    fileSaveDialog(txt: boolean =true, loc: boolean = true){
        let type=""
        if (txt && loc)
            type="txt,loc";
        else {
            if (loc)
                type="loc";
            else
                type="txt";
        }
        return Axios.get(`${CONFIG.API_URL}/app/file-save-dialog?type=${type}`);
    }
}