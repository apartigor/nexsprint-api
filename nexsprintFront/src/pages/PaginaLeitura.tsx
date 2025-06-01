import React, { ReactNode, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_URL } from '../api/api';
import { Viewer, Worker } from '@react-pdf-viewer/core';
import '@react-pdf-viewer/core/lib/styles/index.css';
import '@react-pdf-viewer/default-layout/lib/styles/index.css';
import { useAuth } from '../auth/AuthContext';

interface Modulo {
  nome: string;
  modulosId: number;
  titulo: string;
  autor: string;
  descricao: string;
  capa_URL: string;
  pdF_Url: string;
  totalPaginas: number;
}

const PaginaLeitura: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [modulos, setLivro] = useState<Modulo | null>(null);
  const [zoom, setZoom] = useState(1.0);
  const { user: nomeUsuarioLogado } = useAuth();
  const [darkMode, setDarkMode] = useState(true);
  const [carregandoArquivo, setCarregandoArquivo] = useState(true);
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [highestPage, setHighestPage] = useState(1);


  const progressoPorcentagem = (modulos && modulos.totalPaginas > 0)
    ? (highestPage / modulos.totalPaginas) * 100
    : 0;


  useEffect(() => {
    axios.get(`${API_URL}/modulos/${id}`)
      .then(resp => {
        setLivro(resp.data);
        if (nomeUsuarioLogado && nomeUsuarioLogado !== 'null' && nomeUsuarioLogado.trim() !== '') {
          return axios.get<number>(`${API_URL}/progresso/${id}/${nomeUsuarioLogado}`);
        } else {
          return Promise.reject("Nome de usuário não encontrado.");
        }
      })
      .then(respProgresso => {
        if (respProgresso && typeof respProgresso.data === 'number') {
          const p = respProgresso.data;
          setPaginaAtual(p);
          setHighestPage(p);
        } else if (!nomeUsuarioLogado || nomeUsuarioLogado === 'null' || nomeUsuarioLogado.trim() === '') {
          setPaginaAtual(1);
          setHighestPage(1);
        }
      })
      .catch((err) => {
        console.error("Erro ao buscar dados ou progresso:", err);
        setPaginaAtual(1);
        setHighestPage(1);
      });
  }, [id, nomeUsuarioLogado]);

  const salvarProgresso = (pagina: number) => {
    if (!nomeUsuarioLogado) return; // sem usuário, não salva

    axios
      .post(`${API_URL}/progresso/${id}/${nomeUsuarioLogado}`, { pagina })
      .catch(err => console.error('Erro ao salvar progresso:', err));
  };

  const handlePageChange = (e: { currentPage: number }) => {
    const novaPagina = e.currentPage + 1;
    setPaginaAtual(novaPagina);

    if (novaPagina > highestPage) {
      setHighestPage(novaPagina);
      salvarProgresso(novaPagina);
    }
  };


  useEffect(() => {
    const onUnload = () => {
      if (nomeUsuarioLogado && nomeUsuarioLogado.trim() !== '') {
        salvarProgresso(highestPage);
      }
    };

    window.addEventListener('beforeunload', onUnload);
    window.addEventListener('popstate', onUnload);

    return () => {
      window.removeEventListener('beforeunload', onUnload);
      window.removeEventListener('popstate', onUnload);
    };
  }, [highestPage, nomeUsuarioLogado]);

  const toggleTema = () => setDarkMode(prev => !prev);
  const aumentarZoom = () => setZoom(prev => Math.min(prev + 0.2, 2));
  const diminuirZoom = () => setZoom(prev => Math.max(prev - 0.2, 0.5));
  const onArquivoLoad = () => setCarregandoArquivo(false);

  if (!modulos) {
    return (
      <div className="text-center p-8 text-white dark:text-gray-900">
        Carregando...
      </div>
    );
  }

  const isPDF = modulos?.pdF_Url?.toLowerCase().endsWith('.pdf');
  const temaClasses = darkMode ? 'bg-white text-gray-900' : 'bg-zinc-900 text-white';

  return (
    <div className={`${temaClasses} min-h-screen transition-all duration-300 p-6 relative`}>
      {/* Botões */}
      <div className="fixed right-4 top-1/4 flex flex-col gap-4 z-50">
        <button onClick={aumentarZoom} className="bg-[#023f81] text-black p-3 rounded-full shadow-md hover:bg-[#023f81] transition-all">
          🔍+
        </button>
        <button onClick={diminuirZoom} className="bg-[#023f81] text-black p-3 rounded-full shadow-md hover:bg-[#023f81] transition-all">
          🔍−
        </button>
        <button onClick={toggleTema} className="bg-blue-600 text-white p-3 rounded-full shadow-md hover:bg-blue-500 transition-all">
          {darkMode ? '🌞' : '🌙'}
        </button>
      </div>

      <div className="mb-6 flex justify-between items-center">
        <button
          onClick={() => navigate(-1)}
          className="bg-[#023f81] text-gray-900 font-bold py-2 px-4 rounded hover:bg-[#023f81]"
        >
          ← Voltar
        </button>
      </div>

      <header className="mb-6">
        <h1 className="text-3xl font-bold">{modulos.nome}</h1>
        <p className="mt-2 italic">{modulos.descricao}</p>
        <p className="mt-1 text-sm">Total de páginas: {modulos.totalPaginas}</p>
      </header>

      <div className="fixed bottom-0 left-6 flex flex-col space-y-1 z-50 text-sm text-white-600">
        <span>Página {paginaAtual} de {modulos.totalPaginas}</span>
        <span>{progressoPorcentagem.toFixed(0)}% lido</span>
      </div>
      <div className="fixed top-0 left-0 h-full w-3 bg-white rounded-r-lg overflow-hidden z-50">
        <div
          className="bg-blue-600 transition-all duration-300 w-full"
          style={{ height: `${progressoPorcentagem}%` }}
        ></div>
      </div>

      <div className="flex flex-col md:flex-row gap-6">
        <div className="md:w-1/6 w-full">
          <img src={`http://localhost:5276/${modulos.capa_URL}`} alt="Capa" className="rounded shadow-lg" />
        </div>
        <div className="relative w-1/2">

          {isPDF ? (
            <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.11.174/build/pdf.worker.min.js">
              <div
                className="rounded shadow-xl"
                style={{ transform: `scale(${zoom})`, transformOrigin: 'top left' }}
              >
                <Viewer
                  fileUrl={`http://localhost:5276/${modulos.pdF_Url}`}
                  onDocumentLoad={onArquivoLoad}
                  defaultScale={zoom}
                  initialPage={paginaAtual - 1}
                  onPageChange={handlePageChange}
                />
              </div>
            </Worker>

          ) : (
            <img
              src={`http://localhost:5276${modulos.pdF_Url}`}
              onLoad={onArquivoLoad}
              alt="Modulo"
              style={{ width: `${zoom * 100}%`, height: 'auto' }}
              className="rounded shadow-xl"
            />
          )}

          <a
            href={`http://localhost:5276/${modulos.pdF_Url}`}
            download
            className="inline-block mt-6 bg-[#023f81] text-white font-semibold px-6 py-3 rounded shadow-md hover:bg-[#023f81] transition-all"
          >
            ⬇️ Baixar Arquivo
          </a>
        </div>
      </div>
    </div>
  );
};

export default PaginaLeitura;