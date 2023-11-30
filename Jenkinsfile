pipeline {
  agent any
  stages {
    stage('') {
      steps {
        script {
          stage ('Clean workspace') {
            steps {
              cleanWs()
            }
          }
          stage ('Git Checkout') {
            steps {
              git branch: 'epreuve_certificative_2022_2023',
              credentialsId: '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',
              url: 'https://github.com/ichmat/epreuve_certificative_2022_2023'
            }
          }
          stage('Restore packages') {
            steps {
              bat "dotnet restore ${workspace}\\epreuve_certificative_2022_2023\\FreshTech.sln"
            }
          }
          stage('Clean') {
            steps {
              bat "msbuild.exe ${workspace}\\epreuve_certificative_2022_2023\\FreshTech.sln /nologo /nr:false /p:platform=\"x64\" /p:configuration=\"release\" /t:clean"
            }
          }
          stage('Build') {
            steps {
              bat "msbuild.exe ${workspace}\\epreuve_certificative_2022_2023\\FreshTech.sln /nologo /nr:false  /p:platform=\"x64\" /p:configuration=\"release\" /t:clean;restore;rebuild"
            }
          }
        }

      }
    }

  }
}