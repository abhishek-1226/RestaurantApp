pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOCKER_IMAGE_BACKEND = 'restaurantapp-backend'
        DOCKER_IMAGE_FRONTEND = 'restaurantapp-frontend'
        BUILD_TAG = "${env.BUILD_NUMBER ?: 'latest'}"
        SONAR_HOST_URL = 'http://host.docker.internal:9000'
    }

    stages {

        // ============================
        // Stage 1: BUILD
        // ============================
        stage('Build') {
            parallel {
                stage('Build Backend') {
                    steps {
                        echo '=== Building .NET Backend ==='
                        dir('Backend') {
                            bat 'dotnet restore'
                            bat 'dotnet build -c Release --no-restore'
                            bat 'dotnet publish -c Release -o ./publish --no-build'
                        }
                    }
                }
                stage('Build Frontend') {
                    steps {
                        echo '=== Building React Frontend ==='
                        dir('Frontend') {
                            bat 'npm ci'
                            bat 'npm run build'
                        }
                    }
                }
            }
        }

        // ============================
        // Stage 2: TEST
        // ============================
        stage('Test') {
            steps {
                echo '=== Running .NET Unit Tests ==='
                dir('Backend.Tests') {
                    bat 'dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage" --results-directory ./TestResults'
                }
            }
            post {
                always {
                    junit allowEmptyResults: true,
                          testResults: 'Backend.Tests/TestResults/*.trx'
                }
            }
        }

        // ============================
        // Stage 3: CODE QUALITY
        // ============================
        stage('Code Quality') {
            steps {
                echo '=== Running SonarQube Analysis ==='
                withSonarQubeEnv('SonarQube') {
                    dir('Backend') {
                        sh '''
                            dotnet sonarscanner begin \
                                /k:"RestaurantApp" \
                                /d:sonar.cs.opencover.reportsPaths="../Backend.Tests/TestResults/**/coverage.opencover.xml" \
                                /d:sonar.exclusions="**/Migrations/**,**/obj/**,**/bin/**"
                            dotnet build -c Release
                            dotnet sonarscanner end
                        '''
                    }
                }
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: false
                }
            }
        }

        // ============================
        // Stage 4: SECURITY
        // ============================
        stage('Security') {
            parallel {
                stage('Dependency Check') {
                    steps {
                        echo '=== Running OWASP Dependency Check ==='
                        dependencyCheck additionalArguments: '''
                            --scan ./Backend
                            --scan ./Frontend
                            --format HTML
                            --format JSON
                            --out ./dependency-check-report
                        ''', odcInstallation: 'OWASP-DependencyCheck'
                        dependencyCheckPublisher pattern: 'dependency-check-report/dependency-check-report.json',
                            failedTotalCritical: 10,
                            failedTotalHigh: 20,
                            unstableTotalMedium: 30
                    }
                }
                stage('Container Scan') {
                    steps {
                        echo '=== Running Trivy Container Scan ==='
                        sh '''
                            docker build -t ${DOCKER_IMAGE_BACKEND}:${BUILD_TAG} ./Backend
                            docker build -t ${DOCKER_IMAGE_FRONTEND}:${BUILD_TAG} ./Frontend
                            trivy image --exit-code 0 --severity HIGH,CRITICAL --format table ${DOCKER_IMAGE_BACKEND}:${BUILD_TAG}
                            trivy image --exit-code 0 --severity HIGH,CRITICAL --format table ${DOCKER_IMAGE_FRONTEND}:${BUILD_TAG}
                        '''
                    }
                }
            }
        }

        // ============================
        // Stage 5: DEPLOY
        // ============================
        stage('Deploy') {
            steps {
                echo '=== Deploying to Staging via Docker Compose ==='
                bat 'docker-compose down --remove-orphans || exit 0'
                bat 'docker-compose build --no-cache'
                bat 'docker-compose up -d'
                echo 'Waiting for services to start...'
                sleep(time: 20, unit: 'SECONDS')
                bat 'curl -f http://localhost:5000/health || echo Backend health check pending'
                bat 'curl -f http://localhost:3000/ || echo Frontend health check pending'
            }
        }

        // ============================
        // Stage 6: RELEASE
        // ============================
        stage('Release') {
            steps {
                echo '=== Tagging Release Images ==='
                bat "docker tag %DOCKER_IMAGE_BACKEND%:%BUILD_TAG% %DOCKER_IMAGE_BACKEND%:release-%BUILD_TAG%"
                bat "docker tag %DOCKER_IMAGE_FRONTEND%:%BUILD_TAG% %DOCKER_IMAGE_FRONTEND%:release-%BUILD_TAG%"
                bat "docker tag %DOCKER_IMAGE_BACKEND%:%BUILD_TAG% %DOCKER_IMAGE_BACKEND%:latest"
                bat "docker tag %DOCKER_IMAGE_FRONTEND%:%BUILD_TAG% %DOCKER_IMAGE_FRONTEND%:latest"
                echo "Release tagged as release-%BUILD_TAG% and latest"
            }
        }

        // ============================
        // Stage 7: MONITORING
        // ============================
        stage('Monitoring') {
            steps {
                echo '=== Verifying Deployment Health ==='
                sh '''
                    echo "--- Backend Health Check ---"
                    curl -sf http://localhost:5000/health && echo "Backend: HEALTHY" || echo "Backend: UNHEALTHY"

                    echo "--- Frontend Health Check ---"
                    curl -sf http://localhost:3000/ && echo "Frontend: HEALTHY" || echo "Frontend: UNHEALTHY"

                    echo "--- API Endpoint Smoke Test ---"
                    curl -sf http://localhost:5000/api/restaurants && echo "API Restaurants: OK" || echo "API Restaurants: FAILED"

                    echo "--- Container Status ---"
                    docker-compose ps

                    echo "--- Resource Usage ---"
                    docker stats --no-stream --format "table {{.Name}}\\t{{.CPUPerc}}\\t{{.MemUsage}}" || true
                '''
            }
        }
    }

    post {
        always {
            echo '=== Pipeline Complete ==='
            archiveArtifacts artifacts: 'Backend/publish/**/*', allowEmptyArchive: true
            archiveArtifacts artifacts: 'Frontend/dist/**/*', allowEmptyArchive: true
            archiveArtifacts artifacts: 'dependency-check-report/**/*', allowEmptyArchive: true
        }
        success {
            echo 'Pipeline succeeded!'
        }
        failure {
            echo 'Pipeline failed!'
        }
        cleanup {
            cleanWs()
        }
    }
}