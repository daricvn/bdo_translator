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

    regex(pattern: string, replaceTo: string, ignoreSensitive: boolean= false){
        return Axios.post(`${CONFIG.API_URL}/regex`,{
            pattern,
            text: replaceTo,
            ignoreSensitive: ignoreSensitive?1:0
        });
    }

    find(pattern: string, currentIndex: number, exact: boolean = false, direction: 'up' | 'down' = 'down'){
        return Axios.post(`${CONFIG.API_URL}/findIndex`,{
            pattern,
            currentIndex,
            exact: exact ? 1: 0,
            direction
        });
    }

    writeToFile(filePath: string){
        return Axios.post(`${CONFIG.API_URL}/app/save`, filePath);
    }

    closeApp(){
        return Axios.post(`${CONFIG.API_URL}/app/close`);
    }
}