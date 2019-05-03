import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-player-search-criteria',
  templateUrl: './player-search-criteria.component.html',
  styleUrls: ['./player-search-criteria.component.css']
})
export class PlayerSearchCriteriaComponent implements OnInit {

  searchForm: FormGroup;

  // search criterias
  country: string;
  league: string;
  minimumAge: number;
  maximumAge: number;
  primaryPosition: string;
  secondaryPosition: string;
  injuryStatus: string;
  handPreference: string;
  minimumHeight: number;
  maximumWeight: number;

  // predefined options
  weightList: number[] = [
    50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,
    87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,123,124,125,126,127,
    128,129,130,131,132,133,134,135,136,137,138,139,140];
  heightList: number[] = [
    150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,
    178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200];
  strenghtsList: string[] = ['All','str1', 'str2', 'str3', 'str4', 'str5', 'str6', 'str7', 'str8'];
  weaknessesList: string[] = ['All', 'weak1', 'weak2', 'weak3', 'weak4', 'weak5', 'weak6', 'weak7', 'weak8'];
  injuryStatusList: string[] = ['Both', 'Injured', 'Healthy']
  handPreferenceList: string[] = ['None', 'Left Hand', 'Right Hand', 'Both Hands'];
  positionList: string[] = ['None', 'Left Wing', 'Left Back', 'Playmaker', 'Pivot', 'Right Back', 'Right Wing', 'Defence'];
  leagueList: string[] = ['All Leagues', 'First League', 'Second League', 'Third League'];
  ageList: number[] = [18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50];
  countryList: string[] = [
    'All Countries',
    "Afghanistan","Albania","Algeria","Andorra","Angola","Anguilla","Antigua &amp; Barbuda","Argentina","Armenia","Aruba","Australia","Austria",
    "Azerbaijan","Bahamas","Bahrain","Bangladesh","Barbados","Belarus","Belgium","Belize","Benin","Bermuda","Bhutan",
    "Bolivia","Bosnia &amp; Herzegovina","Botswana","Brazil","British Virgin Islands","Brunei","Bulgaria","Burkina Faso",
    "Burundi","Cambodia","Cameroon","Canada","Cape Verde","Cayman Islands","Chad","Chile","China","Colombia","Congo",
    "Cook Islands","Costa Rica","Cote D Ivoire","Croatia","Cruise Ship","Cuba","Cyprus","Czech Republic","Denmark","Djibouti",
    "Dominica","Dominican Republic","Ecuador","Egypt","El Salvador","Equatorial Guinea","Estonia","Ethiopia","Falkland Islands",
    "Faroe Islands","Fiji","Finland","France","French Polynesia","French West Indies","Gabon","Gambia","Georgia","Germany",
    "Ghana","Gibraltar","Greece","Greenland","Grenada","Guam","Guatemala","Guernsey","Guinea","Guinea Bissau","Guyana","Haiti",
    "Honduras","Hong Kong","Hungary","Iceland","India","Indonesia","Iran","Iraq","Ireland","Isle of Man","Israel",
    "Italy","Jamaica","Japan","Jersey","Jordan","Kazakhstan","Kenya","Kuwait","Kyrgyz Republic","Laos","Latvia","Lebanon","Lesotho",
    "Liberia","Libya","Liechtenstein","Lithuania","Luxembourg","Macau","Macedonia","Madagascar","Malawi","Malaysia",
    "Maldives","Mali","Malta","Mauritania","Mauritius","Mexico","Moldova","Monaco","Mongolia","Montenegro","Montserrat",
    "Morocco","Mozambique","Namibia","Nepal","Netherlands","Netherlands Antilles","New Caledonia","New Zealand","Nicaragua",
    "Niger","Nigeria","Norway","Oman","Pakistan","Palestine","Panama","Papua New Guinea","Paraguay","Peru","Philippines",
    "Poland","Portugal","Puerto Rico","Qatar","Reunion","Romania","Russia","Rwanda","Saint Pierre &amp; Miquelon",
    "Samoa","San Marino","Satellite","Saudi Arabia","Senegal","Serbia","Seychelles","Sierra Leone","Singapore","Slovakia",
    "Slovenia","South Africa","South Korea","Spain","Sri Lanka","St Kitts &amp; Nevis","St Lucia","St Vincent","St. Lucia",
    "Sudan","Suriname","Swaziland","Sweden","Switzerland","Syria","Taiwan","Tajikistan","Tanzania","Thailand","Timor L'Este","Togo","Tonga",
    "Trinidad &amp; Tobago","Tunisia","Turkey","Turkmenistan","Turks &amp; Caicos","Uganda","Ukraine","United Arab Emirates",
    "United Kingdom","United States","United States Minor Outlying Islands","Uruguay","Uzbekistan","Venezuela","Vietnam","Virgin Islands (US)","Yemen","Zambia","Zimbabwe"
  ];

  constructor(private _formbuilder: FormBuilder) {}

  ngOnInit() {
    this.searchForm = this._formbuilder.group({
      country: [''], league: [''], mininumAge: [''], maximumAge: [''],
      primaryPosition: [''], secondaryPosition: [''], injuryStatus: [''],
      handPreference: [''], minimumHeight: [''], maximumWeight: ['']
    });
  }

  /*
    Connect the input with search service
  */
  searchForPlayers() {
    this.validateSearchCriteria();

    // some call to the searchService
  }

  /*
    Helping method to make sure only filled out 
    inputs are being used
  */
  validateSearchCriteria() {
    if(this.searchForm.value.country != '') {
      this.country = this.searchForm.value.country;
    } else {
      this.country = null;
    }  
    
    if(this.searchForm.value.league != '') {
      this.league = this.searchForm.value.league;
    } else {
      this.league = null;
    }

    if(this.searchForm.value.minimumAge != '') {
      this.minimumAge = this.searchForm.value.minimumAge;
    } else {
      this.minimumAge = null;
    }

    if(this.searchForm.value.maximumAge != '') {
      this.maximumAge = this.searchForm.value.maximumAge;
    } else {
      this.maximumAge = null;
    }

    if(this.searchForm.value.primaryPosition != '') {
      this.primaryPosition = this.searchForm.value.primaryPosition;
    } else {
      this.primaryPosition = null;
    }

    if(this.searchForm.value.secondaryPosition != '') {
      this.secondaryPosition = this.searchForm.value.secondaryPosition;
    } else {
      this.secondaryPosition = null;
    }

    if(this.searchForm.value.injuryStatus != '') {
      this.injuryStatus = this.searchForm.value.injuryStatus;
    } else {
      this.injuryStatus = null;
    }

    if(this.searchForm.value.handPreference != '') {
      this.handPreference = this.searchForm.value.handPreference;
    } else {
      this.handPreference = null;
    }

    if(this.searchForm.value.minimumHeight != '') {
      this.minimumHeight = this.searchForm.value.minimumHeight;
    } else {
      this.minimumHeight = null;
    }

    if(this.searchForm.value.maximumWeight != '') {
      this.maximumWeight = this.searchForm.value.maximumWeight;
    } else {
      this.maximumWeight = null;
    }
  }
  
}
