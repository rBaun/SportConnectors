import { Component, OnInit, ViewChild } from '@angular/core';
import { Player } from 'src/app/models/player.model';
import { loginService } from "src/app/services/loginService";
import { updateService } from "src/app/services/updateService";
import { deleteService } from "src/app/services/deleteService";
import { uploadFilesService} from "src/app/services/uploadFilesService";
import { FormControl, Validators } from '@angular/forms';
import { MyErrorStateMatcher } from 'src/app/front-page/front-page-image/register-player/register-player.component';
import { MatCheckbox } from '@angular/material';
import { NationalTeam } from 'src/app/models/nationalTeam.model';

@Component({
  selector: 'app-update-player',
  templateUrl: './update-player.component.html',
  styleUrls: ['./update-player.component.css']
})
export class UpdatePlayerComponent implements OnInit {

  playerBinding: Player;
  step: number;

  // Validators
  validate = new MyErrorStateMatcher();
  numbersOnlyRegex = /^[0-9]*$/;
  currentPassword = new FormControl("", [
    Validators.required,
    Validators.minLength(6)
  ]);
  password = new FormControl("", [
    Validators.required,
    Validators.minLength(6)
  ]); 
  aTeamNumberControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  bTeamNumberControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  u21NumberControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  u18NumberControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  emailControl = new FormControl("", [Validators.required, Validators.email]);
  passwordControl = new FormControl("", [
    Validators.required,
    Validators.minLength(6)
  ]);
  firstNameControl = new FormControl("", Validators.required);
  lastNameControl = new FormControl("", Validators.required);
  countryControl = new FormControl("", Validators.required);
  dayControl = new FormControl("", [
    Validators.required,
    Validators.minLength(2),
    Validators.maxLength(2),
    Validators.pattern(this.numbersOnlyRegex)
  ]);
  monthControl = new FormControl("", Validators.required);
  yearControl = new FormControl("", [
    Validators.required,
    Validators.minLength(4),
    Validators.maxLength(4),
    Validators.pattern(this.numbersOnlyRegex)
  ]);
  heightControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  weightControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  bodyfatControl = new FormControl(
    "",
    Validators.pattern(this.numbersOnlyRegex)
  );
  primaryPositionCtrl = new FormControl("");
  secondaryPositionCtrl = new FormControl("");
  preferredHandCtrl = new FormControl("");
  leagueCtrl = new FormControl("");
  contractStatusCtrl = new FormControl("");
  contractExpiredCtrl = new FormControl("");
  injuryStatusCtrl = new FormControl("");
  injuryDescriptionCtrl = new FormControl("");
  injuryRecoveryDateCtrl = new FormControl("");
  strengthsCtrl = new FormControl("");
  weaknessesCtrl = new FormControl("");
  currentClubCtrl = new FormControl("");
  currentPrimaryPositionCtrl = new FormControl("");
  currentSecondaryPositionCtrl = new FormControl("");
  accomplishmentsCtrl = new FormControl("");
  statistics = new FormControl("");
  formerClubsCtrl = new FormControl("");
  aTeamAppearancesCtrl = new FormControl("");
  aTeamPositionCtrl = new FormControl("");
  aTeamStatisticsCtrl = new FormControl("");
  u21TeamAppearancesCtrl = new FormControl("");
  u21TeamPositionCtrl = new FormControl("");
  u21TeamStatisticsCtrl = new FormControl("");
  bTeamAppearancesCtrl = new FormControl("");
  bTeamPositionCtrl = new FormControl("");
  bTeamStatisticsCtrl = new FormControl("");
  u18TeamAppearancesCtrl = new FormControl("");
  u18TeamPositionCtrl = new FormControl("");
  u18TeamStatisticsCtrl = new FormControl("");


  // strengths and weaknesses
  @ViewChild("speedy") private speedy: MatCheckbox;
  @ViewChild("athletic") private athletic: MatCheckbox;
  @ViewChild("greatShape") private greatShape: MatCheckbox;
  @ViewChild("quickShots") private quickShots: MatCheckbox;
  @ViewChild("accurateShooter") private accurateShooter: MatCheckbox;
  @ViewChild("tactical") private tactical: MatCheckbox;
  @ViewChild("teamPlayer") private teamPlayer: MatCheckbox;
  @ViewChild("social") private social: MatCheckbox;
  @ViewChild("winAtAllCosts") private winAtAllCosts: MatCheckbox;
  @ViewChild("longRangeShooter") private longRangeShooter: MatCheckbox;
  @ViewChild("slowMoving") private slowMoving: MatCheckbox;
  @ViewChild("badEndurance") private badEndurance: MatCheckbox;
  @ViewChild("historyOfInjuries") private historyOfInjuries: MatCheckbox;
  @ViewChild("badDefencePlayer") private badDefencePlayer: MatCheckbox;

  constructor(
    private loginService: loginService,
    private updateService: updateService,
    private uploadFilesService: uploadFilesService,
    private deleteService: deleteService
    ) { }

  ngOnInit() {
    this.setStep(-1); // start with closed accordions
    this.playerBinding = this.loginService.playerInSession;

    // set strengths and weaknesses
    if(this.playerBinding.strengthList.length > 0) {
      this.checkStrengthBoxes(this.playerBinding.strengthList);
    }
    if(this.playerBinding.weaknessList.length > 0) {
      this.checkWeaknessBoxes(this.playerBinding.weaknessList);
    }
    // set current national team info
    if(this.playerBinding.nationalTeamList.length > 0) {
      this.setNationalTeamInfo();
    }
  }

  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.step++;
  }

  prevStep() {
    this.step--;
  }

  upload = (files, type: string) => {
    if (files.length === 0) {
      return;
    }
    else {
      this.uploadFilesService.uploadFile(files).subscribe(res => {
        if(type === 'profile') {
          this.uploadFilesService.createPath(JSON.stringify(res.body), 'image');
          this.playerBinding.imagePath = this.uploadFilesService.imagePath;
        }
        if(type === 'video') {
          this.uploadFilesService.createPath(JSON.stringify(res.body), 'video');
          this.playerBinding.videoPath = this.uploadFilesService.videoPath;
        }
      });
    }
  }

  updatePlayerInfo() {
    this.updateService.updatePlayerInfo(this.buildPlayerInfo()).subscribe(
      (succes: any) => {
          
      },
      error => {
        if(error.error == "Invalid password") {
          // this.wrongPassword = true;
        }
      });
  }

  updatePlayerAdditionalInfo() {
    this.updateService.updatePlayerAdditionalInfo(this.buildPlayerAdditionalInfo()).subscribe(
      (succes: any) => {
          
      },
      error => {
        
      });
  }

  deletePlayerStrengthsAndWeaknesses() {
    // Delete player strengths and weaknesses
    this.deleteService.deleteStrengthsAndWeaknesses().subscribe(
      (succes:any) => { 
        // Insert new player strengths and weaknesses
        this.updateStrengthsAndWeaknesses();
      },
      error => {
        
      });
  }

  updateStrengthsAndWeaknesses() {
    this.updateService.updateStrengthsAndWeaknesses(this.buildStrengthsAndWeaknesses()).subscribe(
      (succes: any) => {
        
      },
      error => {

      });;
  }


  setNationalTeamInfo() {
    this.playerBinding.nationalTeamList.forEach(nt => {
      if(nt.name === 'A') {
        this.aTeamAppearancesCtrl.setValue(nt.appearances);
        this.aTeamPositionCtrl.setValue(nt.position);
        this.aTeamStatisticsCtrl.setValue(nt.statistic);
      } else if(nt.name === 'B') {
        this.bTeamAppearancesCtrl.setValue(nt.appearances);
        this.bTeamPositionCtrl.setValue(nt.position);
        this.bTeamStatisticsCtrl.setValue(nt.statistic);
      } else if(nt.name === 'U21') {
        this.u21TeamAppearancesCtrl.setValue(nt.appearances);
        this.u21TeamPositionCtrl.setValue(nt.position);
        this.u21TeamStatisticsCtrl.setValue(nt.statistic);
      } else if(nt.name === 'U18') {
        this.u18TeamAppearancesCtrl.setValue(nt.appearances);
        this.u18TeamPositionCtrl.setValue(nt.position);
        this.u18TeamStatisticsCtrl.setValue(nt.statistic);
      }
    });
  }

  checkStrengthBoxes(strengths: string[]) {
    strengths.forEach(str => {
      if(str === 'Speedy') {
        this.speedy.checked = true;
      } else if(str === 'Athletic') {
        this.athletic.checked = true;
      } else if(str === 'Great shape') {
        this.greatShape.checked = true;
      } else if(str === 'Quick shots') {
        this.quickShots.checked = true;
      } else if(str === 'Accurate shooter') {
        this.accurateShooter.checked = true;
      } else if(str === 'Tactical') {
        this.tactical.checked = true;
      } else if(str === 'Teamplayer') {
        this.teamPlayer.checked = true;
      } else if(str === 'Social') {
        this.social.checked = true;
      } else if(str === 'Win at all costs') {
        this.winAtAllCosts.checked = true;
      } else if(str === 'Long range shooter') {
        this.longRangeShooter.checked = true;
      }
    });
  }

  checkWeaknessBoxes(weaknesses: string[]) {
    weaknesses.forEach( weak => {
      if(weak === 'Slow moving') {
        this.slowMoving.checked = true;
      } else if(weak === 'Bad endurance') {
        this.badEndurance.checked = true;
      } else if(weak === 'History of injuries') {
        this.historyOfInjuries.checked = true;
      } else if(weak === 'Bad defence player') {
        this.badDefencePlayer.checked = true;
      }
    })
  }

  buildPlayerInfo() {
    var player = new Player();
    player.email = this.playerBinding.email;
    player.password = this.currentPassword.value == "" ? null : this.currentPassword.value;
    player.newPassword = this.password.value == "" ? null : this.password.value;
    player.firstName = this.firstNameControl.value == "" ? this.playerBinding.firstName : this.firstNameControl.value;
    player.lastName = this.lastNameControl.value == "" ? this.playerBinding.lastName : this.lastNameControl.value;
    player.country = this.countryControl.value == "" ? this.playerBinding.country : this.countryControl.value;
    player.day = this.dayControl.value == "" ? this.playerBinding.day : this.dayControl.value;
    player.month = this.monthControl.value == "" ? this.playerBinding.month : this.monthControl.value;
    player.year = this.yearControl.value == "" ? this.playerBinding.year : this.yearControl.value;
    return player;
  }

  buildPlayerAdditionalInfo() {
    var player = new Player();
    player.height = this.heightControl.value == "" ? this.playerBinding.height : this.heightControl.value;
    player.weight = this.weightControl.value == "" ? this.playerBinding.weight : this.weightControl.value;
    player.bodyfat = this.bodyfatControl.value == "" ? this.playerBinding.bodyfat : this.bodyfatControl.value;
    player.primaryPosition = this.primaryPositionCtrl.value == "" ? this.playerBinding.primaryPosition : this.primaryPositionCtrl.value;
    player.secondaryPosition = this.secondaryPositionCtrl.value == "" ? this.playerBinding.secondaryPosition : this.secondaryPositionCtrl.value;
    player.preferredHand = this.preferredHandCtrl.value == "" ? this.playerBinding.preferredHand : this.preferredHandCtrl.value;
    player.league = this.leagueCtrl.value == "" ? this.playerBinding.league : this.leagueCtrl.value;
    player.contractStatus = this.contractStatusCtrl.value == "" ? this.playerBinding.contractStatus : this.contractStatusCtrl.value;
    player.contractExpired = this.contractExpiredCtrl.value == "" ? this.playerBinding.contractExpired : this.buildDate(this.contractExpiredCtrl.value);
    player.injuryStatus = this.injuryStatusCtrl.value == "" ? this.playerBinding.injuryStatus : this.injuryStatusCtrl.value;
    player.injuryDescription = this.injuryDescriptionCtrl.value == "" ? this.playerBinding.injuryDescription : this.injuryDescriptionCtrl.value;
    player.injuryExpired = this.injuryRecoveryDateCtrl.value == "" ? this.playerBinding.injuryExpired : this.buildDate(this.injuryRecoveryDateCtrl.value);
    return player;
  }

  buildStrengthsAndWeaknesses() {
    var player = new Player();
    player.weaknessDescription = this.weaknessesCtrl.value == "" ? this.playerBinding.weaknessDescription : this.weaknessesCtrl.value;
    player.strengthDescription = this.strengthsCtrl.value == "" ? this.playerBinding.strengthDescription : this.strengthsCtrl.value;
    if(this.speedy.checked) {
      player.strengthList.push(this.speedy.value);
    }
    if(this.athletic.checked) {
      player.strengthList.push(this.athletic.value);
    }
    if(this.greatShape.checked) {
      player.strengthList.push(this.greatShape.value);
    }
    if(this.quickShots.checked) {
      player.strengthList.push(this.quickShots.value);
    }
    if(this.accurateShooter.checked) {
      player.strengthList.push(this.accurateShooter.value);
    }
    if(this.tactical.checked) {
      player.strengthList.push(this.tactical.value);
    }
    if(this.teamPlayer.checked) {
      player.strengthList.push(this.teamPlayer.value);
    }
    if(this.social.checked) {
      player.strengthList.push(this.social.value);
    }
    if(this.winAtAllCosts.checked) {
      player.strengthList.push(this.winAtAllCosts.value);
    }
    if(this.longRangeShooter.checked) {
      player.strengthList.push(this.longRangeShooter.value);
    }
    if(this.slowMoving.checked) {
      player.weaknessList.push(this.slowMoving.value);
    }
    if(this.badEndurance.checked) {
      player.weaknessList.push(this.badEndurance.value);
    }
    if(this.historyOfInjuries.checked) {
      player.weaknessList.push(this.historyOfInjuries.value);
    }
    if(this.badDefencePlayer.checked) {
      player.weaknessList.push(this.badDefencePlayer.value);
    }
    return player;
  }

   buildSportCv() {
     this.playerBinding.currentClub = this.currentClubCtrl.value;
     this.playerBinding.currentClubPrimaryPosition = this.currentPrimaryPositionCtrl.value;
     this.playerBinding.currentClubSecondaryPosition = this.currentSecondaryPositionCtrl.value;
     this.playerBinding.accomplishments = this.accomplishmentsCtrl.value;
     this.playerBinding.statistic = this.statistics.value;
     this.playerBinding.formerClubs = this.formerClubsCtrl.value;
   }

   buildNationalTeam() {
     if(this.aTeamAppearancesCtrl.value != '' || this.aTeamPositionCtrl.value != '' || this.aTeamStatisticsCtrl.value != '') {
       let aTeam = new NationalTeam();
       aTeam.name = 'A';
       aTeam.appearances = this.aTeamAppearancesCtrl.value;
       aTeam.position = this.aTeamPositionCtrl.value;
       aTeam.statistic = this.aTeamStatisticsCtrl.value;
     }
     if(this.bTeamAppearancesCtrl.value != '' || this.bTeamPositionCtrl.value != '' || this.bTeamStatisticsCtrl.value != '') {
      let bTeam = new NationalTeam();
      bTeam.name = 'B';
      bTeam.appearances = this.bTeamAppearancesCtrl.value;
      bTeam.position = this.bTeamPositionCtrl.value;
      bTeam.statistic = this.bTeamStatisticsCtrl.value;
    }
    if(this.u21TeamAppearancesCtrl.value != '' || this.u21TeamPositionCtrl.value != '' || this.u21TeamStatisticsCtrl.value != '') {
      let u21Team = new NationalTeam();
      u21Team.name = 'U21';
      u21Team.appearances = this.u21TeamAppearancesCtrl.value;
      u21Team.position = this.u21TeamPositionCtrl.value;
      u21Team.statistic = this.u21TeamStatisticsCtrl.value;
    }
    if(this.u18TeamAppearancesCtrl.value != '' || this.u18TeamPositionCtrl.value != '' || this.u18TeamStatisticsCtrl.value != '') {
      let u18Team = new NationalTeam();
      u18Team.name = 'U18';
      u18Team.appearances = this.u18TeamAppearancesCtrl.value;
      u18Team.position = this.u18TeamPositionCtrl.value;
      u18Team.statistic = this.u18TeamStatisticsCtrl.value;
    }

   }

  // Helping method used to return the date as a string in the format of DD/MM/YYYY
  buildDate(date: Date) {
    return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
  }

  positionList: string[] = [
    "Goalkeeper",
    "Left wing",
    "Left back",
    "Playmaker",
    "Pivot",
    "Right back",
    "Right wing",
    "Defence"
  ];
  countryList: string[] = [
    "Denmark",
    "Norway",
    "Sweden",
    "Afghanistan",
    "Albania",
    "Algeria",
    "Andorra",
    "Angola",
    "Anguilla",
    "Antigua &amp; Barbuda",
    "Argentina",
    "Armenia",
    "Aruba",
    "Australia",
    "Austria",
    "Azerbaijan",
    "Bahamas",
    "Bahrain",
    "Bangladesh",
    "Barbados",
    "Belarus",
    "Belgium",
    "Belize",
    "Benin",
    "Bermuda",
    "Bhutan",
    "Bolivia",
    "Bosnia &amp; Herzegovina",
    "Botswana",
    "Brazil",
    "British Virgin Islands",
    "Brunei",
    "Bulgaria",
    "Burkina Faso",
    "Burundi",
    "Cambodia",
    "Cameroon",
    "Canada",
    "Cape Verde",
    "Cayman Islands",
    "Chad",
    "Chile",
    "China",
    "Colombia",
    "Congo",
    "Cook Islands",
    "Costa Rica",
    "Cote D Ivoire",
    "Croatia",
    "Cruise Ship",
    "Cuba",
    "Cyprus",
    "Czech Republic",
    "Denmark",
    "Djibouti",
    "Dominica",
    "Dominican Republic",
    "Ecuador",
    "Egypt",
    "El Salvador",
    "Equatorial Guinea",
    "Estonia",
    "Ethiopia",
    "Falkland Islands",
    "Faroe Islands",
    "Fiji",
    "Finland",
    "France",
    "French Polynesia",
    "French West Indies",
    "Gabon",
    "Gambia",
    "Georgia",
    "Germany",
    "Ghana",
    "Gibraltar",
    "Greece",
    "Greenland",
    "Grenada",
    "Guam",
    "Guatemala",
    "Guernsey",
    "Guinea",
    "Guinea Bissau",
    "Guyana",
    "Haiti",
    "Honduras",
    "Hong Kong",
    "Hungary",
    "Iceland",
    "India",
    "Indonesia",
    "Iran",
    "Iraq",
    "Ireland",
    "Isle of Man",
    "Israel",
    "Italy",
    "Jamaica",
    "Japan",
    "Jersey",
    "Jordan",
    "Kazakhstan",
    "Kenya",
    "Kuwait",
    "Kyrgyz Republic",
    "Laos",
    "Latvia",
    "Lebanon",
    "Lesotho",
    "Liberia",
    "Libya",
    "Liechtenstein",
    "Lithuania",
    "Luxembourg",
    "Macau",
    "Macedonia",
    "Madagascar",
    "Malawi",
    "Malaysia",
    "Maldives",
    "Mali",
    "Malta",
    "Mauritania",
    "Mauritius",
    "Mexico",
    "Moldova",
    "Monaco",
    "Mongolia",
    "Montenegro",
    "Montserrat",
    "Morocco",
    "Mozambique",
    "Namibia",
    "Nepal",
    "Netherlands",
    "Netherlands Antilles",
    "New Caledonia",
    "New Zealand",
    "Nicaragua",
    "Niger",
    "Nigeria",
    "Norway",
    "Oman",
    "Pakistan",
    "Palestine",
    "Panama",
    "Papua New Guinea",
    "Paraguay",
    "Peru",
    "Philippines",
    "Poland",
    "Portugal",
    "Puerto Rico",
    "Qatar",
    "Reunion",
    "Romania",
    "Russia",
    "Rwanda",
    "Saint Pierre &amp; Miquelon",
    "Samoa",
    "San Marino",
    "Satellite",
    "Saudi Arabia",
    "Senegal",
    "Serbia",
    "Seychelles",
    "Sierra Leone",
    "Singapore",
    "Slovakia",
    "Slovenia",
    "South Africa",
    "South Korea",
    "Spain",
    "Sri Lanka",
    "St Kitts &amp; Nevis",
    "St Lucia",
    "St Vincent",
    "St. Lucia",
    "Sudan",
    "Suriname",
    "Swaziland",
    "Sweden",
    "Switzerland",
    "Syria",
    "Taiwan",
    "Tajikistan",
    "Tanzania",
    "Thailand",
    "Timor L'Este",
    "Togo",
    "Tonga",
    "Trinidad &amp; Tobago",
    "Tunisia",
    "Turkey",
    "Turkmenistan",
    "Turks &amp; Caicos",
    "Uganda",
    "Ukraine",
    "United Arab Emirates",
    "United Kingdom",
    "United States",
    "United States Minor Outlying Islands",
    "Uruguay",
    "Uzbekistan",
    "Venezuela",
    "Vietnam",
    "Virgin Islands (US)",
    "Yemen",
    "Zambia",
    "Zimbabwe"
  ];
}

