import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { API_URL } from '../api/api';

interface Livro {
  moduloId: number;
  nome: string;
  descricao: string;
  capa_URL: string;
  pdF_Url: string;
  totalPaginas: number;
}

interface LivroUsuario extends Livro {
  progresso: number;
}

const Catalogo: React.FC = () => {
  const [books, setBooks] = useState<Livro[]>([]);
  const [userBooks, setUserBooks] = useState<LivroUsuario[]>([]);
  const [search, setSearch] = useState('');

  const user = localStorage.getItem('bytelib_user') || '';

  // Carregar catálogo geral
  useEffect(() => {
    axios.get(`${API_URL}/modulos`)
      .then(response => setBooks(response.data))
      .catch(error => console.error('Erro ao buscar livros:', error));
  }, []);

  // Carregar livros do usuário
  useEffect(() => {
    if (!user) return;
    axios.get(`${API_URL}/meus-modulos/${user}`)
      .then(response => setUserBooks(response.data))
      .catch(error => console.error('Erro ao buscar livros do usuário:', error));
  }, [user]);

  const filteredBooks = books.filter(book =>
    book.nome.toLowerCase().includes(search.toLowerCase()) 
  );

  const filteredUserBooks = userBooks.filter(book =>
    book.nome.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="bg-white text-black min-h-screen p-6">
      <header className="flex items-center justify-between mb-8">
        <img src="/logo.webp" alt="ByteLib" className="h-20" />
        <button
          onClick={() => localStorage.removeItem('bytelib_user')}
          className="text-[#023f81] hover:text-[#023f81] font-semibold transition"
        >
          Sair
        </button>
      </header>

      <h2 className="text-3xl font-bold mb-4">📚 Módulos</h2>

      <div className="mb-8">
        <input
          type="text"
          placeholder="Buscar por título do módulo..."
          className="w-full md:w-1/2 p-3 rounded bg-white text-black border-1 border-[#023f81] placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-[#023f81]"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-6 xl:grid-cols-6 gap-6">
        {filteredBooks.map(book => (
          <div
            key={book.moduloId}
            className="bg-[#023f81] rounded-xl p-4 shadow-md hover:shadow-black transition-all flex flex-col justify-between"
          >
            <img
              src={`http://localhost:5276/${book.capa_URL}`}
              alt={book.nome}
              className="w-full h-64 object-cover rounded-lg mb-4"
            />
            <h3 className="text-xl font-semibold text-white">{book.nome}</h3>
            <p className="text-sm text-gray-300 mb-2 line-clamp-3">{book.descricao}</p>

            <div className="mt-4 flex justify-between items-center">
              <Link
                to={`/modulos/${book.moduloId}`}
                className="bg-black text-white px-4 py-2 rounded font-semibold hover:bg-[#021581] transition"
              >
                📖 Ler
              </Link>

              <a
                href={`http://localhost:5276/${book.pdF_Url}`}
                target="_blank"
                rel="noopener noreferrer"
                className="text-sm text-white hover:underline"
              >
                Baixar
              </a>
            </div>
          </div>
        ))}
      </div>

      {filteredBooks.length === 0 && (
        <p className="text-gray-400 mt-10 text-center">
          Nenhum Módulo encontrado no catálogo com esse termo.
        </p>
      )}

      {/* ========================= */}
      {/* 🎯 Seção Meus modulos */}
      {/* ========================= */}

      <h2 className="text-3xl font-bold mb-4 mt-16">📖 Módulos em Progresso</h2>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-6 xl:grid-cols-6 gap-6">
        {filteredUserBooks.map(book => {
          const progressoPorcentagem = (book.totalPaginas > 0)
            ? (book.progresso / book.totalPaginas) * 100
            : 0;
          return (
            <div
              key={book.moduloId}
              className="bg-zinc-800 rounded-xl p-4 shadow-md hover:shadow-yellow-500/30 transition-all flex flex-col justify-between"
            >
            <img
              src={`http://localhost:5276/${book.capa_URL}`}
              alt={book.nome}
              className="w-full h-64 object-cover rounded-lg mb-4"
            />
            <h3 className="text-xl font-semibold">{book.nome}</h3>
            <p className="text-sm text-gray-300 mb-2 line-clamp-3">{book.descricao}</p>
            <p className="text-sm text-gray-400">Páginas: {book.totalPaginas}</p>

            <div className="w-full bg-zinc-700 rounded-full h-3 mt-2 mb-4">
              <div
                className="bg-[#023f81] h-3 rounded-full"
                style={{ width: `${progressoPorcentagem.toFixed(0)}%` }}
              ></div>
            </div>
            <p className="text-xs text-gray-400 mb-2">Progresso: {progressoPorcentagem.toFixed(0)}%</p>

            <div className="mt-2 flex justify-between items-center">
              <Link to={`/modulos/${book.moduloId}`}
                className="bg-[#023f81] text-black px-4 py-2 rounded font-semibold hover:bg-[#020481] transition"
              >
                📖 Ler
              </Link>

              <a
                href={`http://localhost:5276/${book.pdF_Url}`}
                target="_blank"
                rel="noopener noreferrer"
                className="text-sm text-[#023f81] hover:underline"
              >
                Baixar
              </a>
            </div>
          </div>
          );
        })}
      </div>

      {filteredUserBooks.length === 0 && (
        <p className="text-gray-400 mt-10 text-center">
          Você ainda não possui Módulos disponiveis na sua biblioteca.
        </p>
      )}
    </div>
  );
};

export default Catalogo;
